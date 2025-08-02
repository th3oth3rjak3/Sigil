using Sigil.CodeGeneration;
using Sigil.Common;
using Sigil.ErrorHandling;
using Sigil.Interpretation.Builtins;
using Sigil.Lexing;
using Sigil.Parsing;
using Sigil.Parsing.Expressions;
using Sigil.Parsing.Statements;

namespace Sigil.Interpretation;

public class Interpreter : IExpressionVisitor<object?>, IStatementVisitor<object?>, ICompilerBackend
{
    public ErrorHandler ErrorHandler { get; set; }
    public Environment Environment { get; private set; }

    private readonly Dictionary<string, FunctionStatement> _functions = [];
    private readonly Dictionary<string, ICallable> _builtins = [];
    public TextWriter OutputWriter;

    private readonly ICallable[] _registeredBuiltins = [new PrintBuiltin(), new PrintlnBuiltin()];

    public Interpreter(string SourceCode, TextWriter? outputWriter = null)
    {
        ErrorHandler = new ErrorHandler(SourceCode);
        Environment = new Environment();
        OutputWriter = outputWriter ?? Console.Out;

        _registeredBuiltins
        .ToList()
        .ForEach(RegisterBuiltin);
    }

    public void RegisterBuiltin(ICallable builtin)
    {
        _builtins[builtin.Name] = builtin;
    }

    public int Execute(List<Statement> nodes)
    {
        Interpret(nodes);
        return 0;
    }

    public void Interpret(List<Statement> statements)
    {
        try
        {
            foreach (var statement in statements)
            {
                statement.Accept(this);
            }
        }
        catch (ReturnValue returnValue)
        {
            // Handle return at top-level - print the value
            OutputWriter.WriteLine(Stringify(returnValue.Value));
        }
        catch (RuntimeException ex)
        {
            ErrorHandler.Report(ex.Message, ex.Span);
        }
    }

    // Expression visitors
    public object? VisitIntegerLiteralExpression(IntegerLiteralExpression expr)
    {
        return expr.Value;
    }

    public object? VisitFloatLiteralExpression(FloatLiteralExpression expr)
    {
        return expr.Value;
    }

    public object? VisitStringLiteralExpression(StringLiteralExpression expr)
    {
        return expr.Value;
    }

    public object? VisitBooleanLiteralExpression(BooleanLiteralExpression expr)
    {
        return expr.Value;
    }

    public object? VisitCharacterLiteralExpression(CharacterLiteralExpression expression)
    {
        return expression.Value;
    }

    public object? VisitIdentifierExpression(IdentifierExpression expr)
    {
        return Environment.Get(expr.Name, expr.Span);
    }

    public object? VisitBinaryExpression(BinaryExpression expr)
    {
        // For logical operators, we need short-circuiting
        if (expr.Operator.TokenType == TokenType.Or)
        {
            var left = expr.Left.Accept(this);
            if (IsTruthy(left)) return left; // Short-circuit: if left is truthy, return it
            return expr.Right.Accept(this);  // Otherwise evaluate right
        }

        if (expr.Operator.TokenType == TokenType.And)
        {
            var left = expr.Left.Accept(this);
            if (!IsTruthy(left)) return left; // Short-circuit: if left is falsy, return it
            return expr.Right.Accept(this);   // Otherwise evaluate right
        }

        // For other operators, evaluate both sides first
        var leftVal = expr.Left.Accept(this);
        var rightVal = expr.Right.Accept(this);

        return expr.Operator.TokenType switch
        {
            TokenType.Plus => Add(leftVal, rightVal, expr.Span),
            TokenType.Minus => Subtract(leftVal, rightVal, expr.Span),
            TokenType.Star => Multiply(leftVal, rightVal, expr.Span),
            TokenType.Slash => Divide(leftVal, rightVal, expr.Span),
            TokenType.Greater => IsGreater(leftVal, rightVal, expr.Span),
            TokenType.GreaterEqual => IsGreaterEqual(leftVal, rightVal, expr.Span),
            TokenType.Less => IsLess(leftVal, rightVal, expr.Span),
            TokenType.LessEqual => IsLessEqual(leftVal, rightVal, expr.Span),
            TokenType.EqualEqual => IsEqual(leftVal, rightVal),
            TokenType.BangEqual => !IsEqual(leftVal, rightVal),
            _ => throw new RuntimeException($"Unknown binary operator: {expr.Operator.TokenType}", expr.Span)
        };
    }

    public object? VisitUnaryExpression(UnaryExpression expr)
    {
        var right = expr.Right.Accept(this);

        return expr.Operator.TokenType switch
        {
            TokenType.Minus => Negate(right, expr.Span),
            TokenType.Bang => !IsTruthy(right),
            _ => throw new RuntimeException($"Unknown unary operator: {expr.Operator.TokenType}", expr.Span)
        };
    }

    public object? VisitGroupingExpression(GroupingExpression expr)
    {
        return expr.Expression.Accept(this);
    }

    public object? VisitCallExpression(CallExpression expr)
    {
        // For now, assume callee is a function name (identifier)
        if (expr.Callee is not IdentifierExpression identExpr)
            throw new RuntimeException("Can only call functions", expr.Span);


        var name = identExpr.Name;

        if (_builtins.TryGetValue(name, out var builtin))
        {
            if (builtin.Arity >= 0 && expr.Arguments.Count != builtin.Arity)
                throw new RuntimeException(
                    $"Builtin '{name}' expects {builtin.Arity} argument(s), got {expr.Arguments.Count}",
                    expr.Span
                );

            var args = expr.Arguments.Select(arg => arg.Accept(this)).ToList();
            return builtin.Call(this, args, expr.Span);
        }

        if (!_functions.TryGetValue(identExpr.Name, out var function))
            throw new RuntimeException($"Undefined function '{identExpr.Name}'", expr.Span);

        if (expr.Arguments.Count != function.Parameters.Count)
            throw new RuntimeException($"Expected {function.Parameters.Count} arguments but got {expr.Arguments.Count}", expr.Span);

        // Evaluate arguments
        var arguments = expr.Arguments.Select(arg => arg.Accept(this)).ToList();

        // Create new environment for function
        var functionEnv = new Environment(Environment);

        // Bind parameters
        for (var i = 0; i < function.Parameters.Count; i++)
        {
            functionEnv.Define(function.Parameters[i], arguments[i]);
        }

        // Execute function body
        try
        {
            ExecuteBlock(function.Body, functionEnv);
            return null; // No return statement
        }
        catch (ReturnValue returnValue)
        {
            return returnValue.Value;
        }
    }

    // Statement visitors
    public object? VisitExpressionStatement(ExpressionStatement stmt)
    {
        stmt.Expression.Accept(this);
        return null;
    }

    public object? VisitLetStatement(LetStatement stmt)
    {
        var value = stmt.Initializer.Accept(this);
        Environment.Define(stmt.Name, value);
        return null;
    }

    public object? VisitBlockStatement(BlockStatement stmt)
    {
        ExecuteBlock(stmt.Statements, new Environment(Environment));
        return null;
    }

    public object? VisitIfStatement(IfStatement stmt)
    {
        var conditionValue = stmt.Condition.Accept(this);

        if (IsTruthy(conditionValue))
        {
            stmt.ThenBranch.Accept(this);
        }
        else
        {
            stmt.ElseBranch?.Accept(this);
        }

        return null;
    }

    public object? VisitWhileStatement(WhileStatement stmt)
    {
        while (true)
        {
            var conditionValue = stmt.Condition.Accept(this);

            if (!IsTruthy(conditionValue))
                break;

            stmt.Body.Accept(this);
        }

        return null;
    }

    public object? VisitReturnStatement(ReturnStatement stmt)
    {
        var result = stmt.Expression?.Accept(this);

        // If we're in a function, throw the return value
        throw new ReturnValue(result);
    }

    public object? VisitAssignmentStatement(AssignmentStatement stmt)
    {
        var value = stmt.Value.Accept(this);
        Environment.Set(stmt.Name, value, stmt.Span);
        return null;
    }

    public object? VisitFunctionStatement(FunctionStatement stmt)
    {
        _functions[stmt.Name] = stmt;
        return null;
    }

    // Helper methods for operations
    private object Add(object? left, object? right, Span span)
    {
        if (left is long leftInt && right is long rightInt)
            return leftInt + rightInt;

        if (left is double leftDouble && right is double rightDouble)
            return leftDouble + rightDouble;

        if (left is long leftIntF && right is double rightDoubleF)
            return leftIntF + rightDoubleF;

        if (left is double leftDoubleI && right is long rightIntI)
            return leftDoubleI + rightIntI;

        if (left is string leftStr && right is string rightStr)
            return leftStr + rightStr;

        if (left is string sLeft && right is char cRight)
            return sLeft + cRight;

        if (left is char cLeft && right is string sRight)
            return cLeft + sRight;

        if (left is char cLeft2 && right is char cRight2)
            return cLeft2.ToString() + cRight2.ToString();

        throw new RuntimeException($"Cannot add {GetTypeName(left)} and {GetTypeName(right)}", span);
    }

    private object Subtract(object? left, object? right, Span span)
    {
        if (left is long leftInt && right is long rightInt)
            return leftInt - rightInt;

        if (left is double leftDouble && right is double rightDouble)
            return leftDouble - rightDouble;

        if (left is long leftIntF && right is double rightDoubleF)
            return leftIntF - rightDoubleF;

        if (left is double leftDoubleI && right is long rightIntI)
            return leftDoubleI - rightIntI;

        throw new RuntimeException($"Cannot subtract {GetTypeName(right)} from {GetTypeName(left)}", span);
    }

    private object Multiply(object? left, object? right, Span span)
    {
        if (left is long leftInt && right is long rightInt)
            return leftInt * rightInt;

        if (left is double leftDouble && right is double rightDouble)
            return leftDouble * rightDouble;

        if (left is long leftIntF && right is double rightDoubleF)
            return leftIntF * rightDoubleF;

        if (left is double leftDoubleI && right is long rightIntI)
            return leftDoubleI * rightIntI;

        throw new RuntimeException($"Cannot multiply {GetTypeName(left)} and {GetTypeName(right)}", span);
    }

    private object Divide(object? left, object? right, Span span)
    {
        if (left is long leftInt && right is long rightInt)
        {
            if (rightInt == 0)
                throw new RuntimeException("Division by zero", span);
            return leftInt / rightInt;
        }

        if (left is double leftDouble && right is double rightDouble)
        {
            if (rightDouble == 0.0)
                throw new RuntimeException("Division by zero", span);
            return leftDouble / rightDouble;
        }

        if (left is long leftIntF && right is double rightDoubleF)
        {
            if (rightDoubleF == 0.0)
                throw new RuntimeException("Division by zero", span);
            return leftIntF / rightDoubleF;
        }

        if (left is double leftDoubleI && right is long rightIntI)
        {
            if (rightIntI == 0)
                throw new RuntimeException("Division by zero", span);
            return leftDoubleI / rightIntI;
        }

        throw new RuntimeException($"Cannot divide {GetTypeName(left)} by {GetTypeName(right)}", span);
    }

    private static object Negate(object? operand, Span span)
    {
        if (operand is long intVal)
            return -intVal;
        if (operand is double doubleVal)
            return -doubleVal;

        throw new RuntimeException($"Cannot negate {GetTypeName(operand)}", span);
    }

    private bool IsGreater(object? left, object? right, Span span)
    {
        if (left is long leftInt && right is long rightInt)
            return leftInt > rightInt;

        if (left is double leftDouble && right is double rightDouble)
            return leftDouble > rightDouble;

        if (left is long leftIntF && right is double rightDoubleF)
            return leftIntF > rightDoubleF;

        if (left is double leftDoubleI && right is long rightIntI)
            return leftDoubleI > rightIntI;

        throw new RuntimeException($"Cannot compare {GetTypeName(left)} and {GetTypeName(right)}", span);
    }

    private bool IsGreaterEqual(object? left, object? right, Span span)
    {
        if (left is long leftInt && right is long rightInt)
            return leftInt >= rightInt;

        if (left is double leftDouble && right is double rightDouble)
            return leftDouble >= rightDouble;

        if (left is long leftIntF && right is double rightDoubleF)
            return leftIntF >= rightDoubleF;

        if (left is double leftDoubleI && right is long rightIntI)
            return leftDoubleI >= rightIntI;

        throw new RuntimeException($"Cannot compare {GetTypeName(left)} and {GetTypeName(right)}", span);
    }

    private bool IsLess(object? left, object? right, Span span)
    {
        if (left is long leftInt && right is long rightInt)
            return leftInt < rightInt;

        if (left is double leftDouble && right is double rightDouble)
            return leftDouble < rightDouble;

        if (left is long leftIntF && right is double rightDoubleF)
            return leftIntF < rightDoubleF;

        if (left is double leftDoubleI && right is long rightIntI)
            return leftDoubleI < rightIntI;

        throw new RuntimeException($"Cannot compare {GetTypeName(left)} and {GetTypeName(right)}", span);
    }

    private bool IsLessEqual(object? left, object? right, Span span)
    {
        if (left is long leftInt && right is long rightInt)
            return leftInt <= rightInt;

        if (left is double leftDouble && right is double rightDouble)
            return leftDouble <= rightDouble;

        if (left is long leftIntF && right is double rightDoubleF)
            return leftIntF <= rightDoubleF;

        if (left is double leftDoubleI && right is long rightIntI)
            return leftDoubleI <= rightIntI;

        throw new RuntimeException($"Cannot compare {GetTypeName(left)} and {GetTypeName(right)}", span);
    }

    private static bool IsEqual(object? left, object? right)
    {
        if (left == null && right == null) return true;
        if (left == null) return false;
        return left.Equals(right);
    }

    private bool IsTruthy(object? obj)
    {
        if (obj == null) return false;
        if (obj is bool boolean) return boolean;
        return true; // Everything else is truthy
    }

    private static string GetTypeName(object? obj)
    {
        return obj switch
        {
            long => "Int",
            double => "Float",
            string => "String",
            bool => "Bool",
            char => "Char",
            null => "null",
            _ => obj.GetType().Name
        };
    }

    private static string Stringify(object? obj)
    {
        if (obj == null) return "null";
        if (obj is double d)
        {
            var text = d.ToString();
            if (text.EndsWith(".0"))
                text = text[..^2];
            return text;
        }
        return obj.ToString() ?? "null";
    }

    public void ExecuteBlock(List<Statement> statements, Environment environment)
    {
        var previous = Environment;
        try
        {
            // Use the new environment for this block
            Environment = environment;
            foreach (var statement in statements)
            {
                statement.Accept(this);
            }
        }
        finally
        {
            // Restore previous environment
            Environment = previous;
        }
    }
}
