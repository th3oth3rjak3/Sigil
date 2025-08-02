using Sigil.Parsing.Expressions;
using Sigil.Parsing.Statements;
using Sigil.ErrorHandling;
using Sigil.Common;
using Sigil.Lexing;

namespace Sigil.TypeChecking;

// Basic type representations
public abstract record Type
{
    public abstract override string ToString();
};
public record IntType : Type
{
    public override string ToString() => "Int";
}

public record FloatType : Type
{
    public override string ToString() => "Float";
}

public record StringType : Type
{
    public override string ToString() => "String";
}

public record BoolType : Type
{
    public override string ToString() => "Bool";
}

public record CharType : Type
{
    public override string ToString() => "Char";
}

public record VoidType : Type
{
    public override string ToString() => "Void";
}

public record FunctionType(List<Type> ParameterTypes, Type ReturnType) : Type
{
    public override string ToString() => "Function";
}

public record AnyType : Type
{
    public override string ToString() => "Any";
}


public record ErrorType(string Message) : Type // For type errors
{
    public override string ToString() => "Error";
}


public class TypeCheckingVisitor : IStatementVisitor<Type>, IExpressionVisitor<Type>
{
    private readonly Dictionary<string, Type> variables = new();
    private readonly Dictionary<string, FunctionType> functions = new();
    private readonly ErrorHandler errorHandler;

    public TypeCheckingVisitor(ErrorHandler errorHandler)
    {
        this.errorHandler = errorHandler;
    }

    // Helper method to report errors with proper location
    private ErrorType ReportError(string message, Span location)
    {
        errorHandler.Report(message, location);
        return new ErrorType(message);
    }

    // Overload for expressions that have location info
    private ErrorType ReportError(string message, Expression expression)
    {
        return ReportError(message, expression.Span);
    }

    // Overload for statements that have location info
    private ErrorType ReportError(string message, Statement statement)
    {
        return ReportError(message, statement.Span);
    }

    // Helper method to convert type name strings to Type objects
    private Type ParseTypeName(string typeName) => typeName switch
    {
        "Int" => new IntType(),
        "Float" => new FloatType(),
        "String" => new StringType(),
        "Bool" => new BoolType(),
        "Char" => new CharType(),
        "Void" => new VoidType(),
        _ => new ErrorType($"Unknown type: {typeName}") // Don't report here, let caller handle
    };

    // Helper method to get built-in function types
    private FunctionType? GetBuiltInFunction(string name) => name switch
    {
        "println" => new FunctionType(new List<Type> { new StringType() }, new VoidType()),
        "print" => new FunctionType(new List<Type> { new StringType() }, new VoidType()),
        "string" => new FunctionType(new List<Type>() { new AnyType() }, new StringType()),
        _ => null
    };

    // Expression visitors
    public Type VisitBinaryExpression(BinaryExpression expression)
    {
        var leftType = expression.Left.Accept(this);
        var rightType = expression.Right.Accept(this);

        // Handle error propagation
        if (leftType is ErrorType || rightType is ErrorType)
            return leftType is ErrorType ? leftType : rightType;

        var tokenType = expression.Operator.TokenType;

        return tokenType switch
        {
            // Arithmetic operators
            TokenType.Plus or TokenType.Minus or TokenType.Star or TokenType.Slash =>
                CheckArithmeticOperation(leftType, rightType, tokenType, expression),

            // Comparison operators
            TokenType.EqualEqual or TokenType.BangEqual =>
                CheckEqualityOperation(leftType, rightType, expression),
            TokenType.Greater or TokenType.GreaterEqual or TokenType.Less or TokenType.LessEqual =>
                CheckComparisonOperation(leftType, rightType, expression),

            _ => ReportError($"Unknown binary operator: {tokenType}", expression)
        };
    }

    private Type CheckArithmeticOperation(Type left, Type right, TokenType op, BinaryExpression expression)
    {
        return (left, right) switch
        {
            (IntType, IntType) => new IntType(),
            (FloatType, FloatType) => new FloatType(),
            (IntType, FloatType) => new FloatType(),
            (FloatType, IntType) => new FloatType(),
            (StringType, StringType) when op == TokenType.Plus => new StringType(), // String concatenation
            (CharType, StringType) when op == TokenType.Plus => new StringType(),
            (StringType, CharType) when op == TokenType.Plus => new StringType(),
            (CharType, CharType) when op == TokenType.Plus => new StringType(),
            _ => ReportError($"Cannot apply operator '{op}' to types {left} and {right}", expression)
        };
    }

    private Type CheckEqualityOperation(Type left, Type right, BinaryExpression expression)
    {
        // Most types can be compared for equality
        return new BoolType();
    }

    private Type CheckComparisonOperation(Type left, Type right, BinaryExpression expression)
    {
        return (left, right) switch
        {
            (IntType, IntType) => new BoolType(),
            (FloatType, FloatType) => new BoolType(),
            (IntType, FloatType) => new BoolType(),
            (FloatType, IntType) => new BoolType(),
            (StringType, StringType) => new BoolType(),
            _ => ReportError($"Cannot compare types {left} and {right}", expression)
        };
    }

    public Type VisitUnaryExpression(UnaryExpression expression)
    {
        var operandType = expression.Right.Accept(this);

        if (operandType is ErrorType) return operandType;

        var tokenType = expression.Operator.TokenType;

        return tokenType switch
        {
            TokenType.Minus => operandType switch
            {
                IntType => new IntType(),
                FloatType => new FloatType(),
                _ => ReportError($"Cannot apply unary '-' to type {operandType}", expression)
            },
            TokenType.Bang => operandType switch
            {
                BoolType => new BoolType(),
                _ => ReportError($"Cannot apply unary '!' to type {operandType}", expression)
            },
            _ => ReportError($"Unknown unary operator: {tokenType}", expression)
        };
    }

    public Type VisitCallExpression(CallExpression expression)
    {
        var calleeType = expression.Callee.Accept(this);

        // For now, handle identifiers as function calls
        if (expression.Callee is IdentifierExpression id)
        {
            if (functions.TryGetValue(id.Name, out var funcType))
            {
                // Check argument count
                if (expression.Arguments.Count != funcType.ParameterTypes.Count)
                {
                    return ReportError($"Function '{id.Name}' expects {funcType.ParameterTypes.Count} arguments but got {expression.Arguments.Count}", expression);
                }

                // Check argument types
                for (int i = 0; i < expression.Arguments.Count; i++)
                {
                    var argType = expression.Arguments[i].Accept(this);

                    // If the argument itself has a type error, don't report another error here
                    if (argType is ErrorType)
                    {
                        return argType; // Propagate the error from the argument
                    }

                    var expectedType = funcType.ParameterTypes[i];

                    if (!TypesMatch(argType, expectedType))
                    {
                        return ReportError($"Argument {i + 1} to function '{id.Name}' expected {expectedType} but got {argType}", expression.Arguments[i]);
                    }
                }

                return funcType.ReturnType;
            }
            else
            {
                // Check built-in functions
                var builtInFunc = GetBuiltInFunction(id.Name);
                if (builtInFunc != null)
                {
                    // Check argument count
                    if (expression.Arguments.Count != builtInFunc.ParameterTypes.Count)
                    {
                        return ReportError($"Function '{id.Name}' expects {builtInFunc.ParameterTypes.Count} arguments but got {expression.Arguments.Count}", expression);
                    }

                    // Check argument types
                    for (int i = 0; i < expression.Arguments.Count; i++)
                    {
                        var argType = expression.Arguments[i].Accept(this);

                        // If the argument itself has a type error, don't report another error here
                        if (argType is ErrorType)
                        {
                            return argType; // Propagate the error from the argument
                        }

                        var expectedType = builtInFunc.ParameterTypes[i];

                        if (!TypesMatch(argType, expectedType))
                        {
                            return ReportError($"Argument {i + 1} to function '{id.Name}' expected {expectedType} but got {argType}", expression.Arguments[i]);
                        }
                    }

                    return builtInFunc.ReturnType;
                }

                return ReportError($"Unknown function: {id.Name}", expression);
            }
        }

        return ReportError("Invalid function call", expression);
    }

    public Type VisitGroupingExpression(GroupingExpression expression)
    {
        return expression.Expression.Accept(this);
    }

    public Type VisitIdentifierExpression(IdentifierExpression expression)
    {
        // First check if it's a variable
        if (variables.TryGetValue(expression.Name, out var type))
        {
            return type;
        }

        // Then check if it's a function name
        if (functions.TryGetValue(expression.Name, out var funcType))
        {
            return funcType; // Return the function type
        }

        // Check for built-in functions
        var builtInType = GetBuiltInFunction(expression.Name);
        if (builtInType != null)
        {
            return builtInType;
        }

        return ReportError($"Undefined variable or function: {expression.Name}", expression);
    }

    // Literal expressions
    public Type VisitIntegerLiteralExpression(IntegerLiteralExpression expression)
    {
        return new IntType();
    }

    public Type VisitFloatLiteralExpression(FloatLiteralExpression expression)
    {
        return new FloatType();
    }

    public Type VisitStringLiteralExpression(StringLiteralExpression expression)
    {
        return new StringType();
    }

    public Type VisitBooleanLiteralExpression(BooleanLiteralExpression expression)
    {
        return new BoolType();
    }

    public Type VisitCharacterLiteralExpression(CharacterLiteralExpression expression)
    {
        return new CharType();
    }

    // Statement visitors
    public Type VisitAssignmentStatement(AssignmentStatement statement)
    {
        var valueType = statement.Value.Accept(this);

        if (variables.TryGetValue(statement.Name, out var existingType))
        {
            if (!TypesMatch(valueType, existingType))
            {
                return ReportError($"Cannot assign {valueType} to variable '{statement.Name}' of type {existingType}", statement);
            }
        }
        else
        {
            return ReportError($"Undefined variable: {statement.Name}", statement);
        }

        return new VoidType();
    }

    public Type VisitLetStatement(LetStatement statement)
    {
        var initType = statement.Initializer.Accept(this);

        if (initType is not ErrorType)
        {
            // If there's an explicit type annotation, validate it matches
            if (!string.IsNullOrEmpty(statement.TypeName))
            {
                var declaredType = ParseTypeName(statement.TypeName);
                if (declaredType is ErrorType)
                {
                    return ReportError($"Unknown type: {statement.TypeName}", statement);
                }

                if (!TypesMatch(initType, declaredType))
                {
                    return ReportError($"Variable '{statement.Name}' declared as {declaredType} but initialized with {initType}", statement);
                }
                variables[statement.Name] = declaredType;
            }
            else
            {
                // No explicit type - infer from initializer
                variables[statement.Name] = initType;
            }
        }

        return new VoidType();
    }

    public Type VisitFunctionStatement(FunctionStatement statement)
    {
        // Parse parameter types from their type annotations
        var paramTypes = new List<Type>();
        foreach (var param in statement.Parameters)
        {
            var paramType = ParseTypeName(param.TypeName);
            if (paramType is ErrorType)
            {
                ReportError($"Unknown parameter type: {param.TypeName}", statement);
                continue;
            }
            paramTypes.Add(paramType);
        }

        // Parse return type from annotation
        var returnType = ParseTypeName(statement.ReturnTypeName ?? "Void");
        if (returnType is ErrorType)
        {
            ReportError($"Unknown return type: {statement.ReturnTypeName}", statement);
            returnType = new VoidType(); // Use void as fallback
        }

        // Register function before checking body (for recursion)
        var funcType = new FunctionType(paramTypes, returnType);
        functions[statement.Name] = funcType;

        // Create function scope for type checking body
        var oldVariables = new Dictionary<string, Type>(variables);

        // Add parameters to scope with their declared types
        for (int i = 0; i < statement.Parameters.Count && i < paramTypes.Count; i++)
        {
            variables[statement.Parameters[i].Name] = paramTypes[i];
        }

        // Type check function body
        Type actualReturnType = new VoidType();
        foreach (var stmt in statement.Body)
        {
            var stmtType = stmt.Accept(this);
            if (stmt is ReturnStatement)
            {
                actualReturnType = stmtType;
            }
        }

        // Validate that actual return type matches declared return type
        if (!TypesMatch(actualReturnType, returnType))
        {
            ReportError($"Function '{statement.Name}' declared to return {returnType} but actually returns {actualReturnType}", statement);
        }

        // Restore previous scope
        variables.Clear();
        foreach (var kvp in oldVariables)
        {
            variables[kvp.Key] = kvp.Value;
        }

        return new VoidType();
    }

    public Type VisitReturnStatement(ReturnStatement statement)
    {
        if (statement.Expression != null)
        {
            return statement.Expression.Accept(this);
        }
        return new VoidType();
    }

    public Type VisitExpressionStatement(ExpressionStatement statement)
    {
        return statement.Expression.Accept(this);
    }

    public Type VisitBlockStatement(BlockStatement statement)
    {
        Type lastType = new VoidType();
        foreach (var stmt in statement.Statements)
        {
            lastType = stmt.Accept(this);
        }
        return lastType;
    }

    public Type VisitIfStatement(IfStatement statement)
    {
        var conditionType = statement.Condition.Accept(this);

        if (!TypesMatch(conditionType, new BoolType()))
        {
            ReportError($"If condition must be boolean, got {conditionType}", statement);
        }

        statement.ThenBranch.Accept(this);
        statement.ElseBranch?.Accept(this);

        return new VoidType();
    }

    public Type VisitWhileStatement(WhileStatement statement)
    {
        var conditionType = statement.Condition.Accept(this);

        if (!TypesMatch(conditionType, new BoolType()))
        {
            ReportError($"While condition must be boolean, got {conditionType}", statement);
        }

        statement.Body.Accept(this);
        return new VoidType();
    }

    public Type VisitClassStatement(ClassStatement statement)
    {
        // Basic class type checking - you can expand this
        foreach (var method in statement.Methods)
        {
            method.Accept(this);
        }
        return new VoidType();
    }

    // Helper method to check if types match
    private bool TypesMatch(Type actual, Type expected)
    {
        if (actual is ErrorType || expected is ErrorType)
            return false;

        return actual.GetType() == expected.GetType() || expected.GetType() == new AnyType().GetType();
    }

    // Public method to type check a list of statements
    public void TypeCheck(List<Statement> statements)
    {
        // First pass: collect all function declarations
        foreach (var statement in statements)
        {
            if (statement is FunctionStatement funcStmt)
            {
                var paramTypes = new List<Type>();
                foreach (var param in funcStmt.Parameters)
                {
                    var paramType = ParseTypeName(param.TypeName);
                    if (paramType is not ErrorType)
                    {
                        paramTypes.Add(paramType);
                    }
                    else
                    {
                        // Report error with proper location
                        ReportError($"Unknown parameter type: {param.TypeName}", funcStmt);
                        paramTypes.Add(new IntType()); // Fallback
                    }
                }

                var returnType = ParseTypeName(funcStmt.ReturnTypeName ?? "Void");
                if (returnType is ErrorType)
                {
                    ReportError($"Unknown return type: {funcStmt.ReturnTypeName}", funcStmt);
                    returnType = new VoidType(); // Fallback
                }

                var funcType = new FunctionType(paramTypes, returnType);
                functions[funcStmt.Name] = funcType;
            }
        }

        // Second pass: type check all statements
        foreach (var statement in statements)
        {
            statement.Accept(this);
        }
    }
}
