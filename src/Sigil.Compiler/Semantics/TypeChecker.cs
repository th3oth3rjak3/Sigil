namespace Sigil.Compiler.Semantics;

using Sigil.Compiler.Syntax;

public sealed class TypeChecker
{
    public TypedModule Check(BoundModule module)
    {
        var declarations = new List<TypedFunctionDeclaration>(
            module.Declarations.Count);

        foreach (var declaration in module.Declarations)
        {
            declarations.Add(CheckFunction(declaration));
        }

        return new TypedModule(declarations);
    }

    private TypedFunctionDeclaration CheckFunction(
        BoundFunctionDeclaration function)
    {
        var returnType = ResolveType(
            function.Declaration.ReturnType);

        var body = CheckBlock(
            function.Body,
            returnType);

        return new TypedFunctionDeclaration(
            function.Declaration,
            returnType,
            body);
    }

    private TypedBlock CheckBlock(
        BoundBlock block,
        Type functionReturnType)
    {
        var statements = new List<TypedStatement>(
            block.Statements.Count);

        foreach (var statement in block.Statements)
        {
            statements.Add(
                CheckStatement(
                    statement,
                    functionReturnType));
        }

        return new TypedBlock(statements);
    }

    private TypedStatement CheckStatement(
        BoundStatement statement,
        Type functionReturnType)
    {
        return statement switch
        {
            BoundLetStatement let =>
                CheckLet(let),

            BoundReturnStatement @return =>
                CheckReturn(
                    @return,
                    functionReturnType),

            _ => throw new Exception(
                $"Unsupported bound statement: " +
                $"{statement.GetType().Name}.")
        };
    }

    private TypedLetStatement CheckLet(
        BoundLetStatement statement)
    {
        var type = ResolveType(
            statement.Declaration.Type);

        var initializer = CheckExpression(
            statement.Initializer);

        if (!AreSameType(type, initializer.Type))
        {
            throw new Exception(
                $"Cannot assign value of type " +
                $"'{initializer.Type.GetType().Name}' " +
                $"to variable '{statement.Declaration.Name}' " +
                $"of type '{type.GetType().Name}'.");
        }

        return new TypedLetStatement(
            statement.Declaration,
            statement.Variable,
            type,
            initializer);
    }

    private TypedReturnStatement CheckReturn(
        BoundReturnStatement statement,
        Type functionReturnType)
    {
        if (statement.Value is null)
        {
            if (functionReturnType is not VoidType)
            {
                throw new Exception(
                    "A non-void function must return a value.");
            }

            return new TypedReturnStatement(
                statement.Statement,
                null);
        }

        if (functionReturnType is VoidType)
        {
            throw new Exception(
                "A void function cannot return a value.");
        }

        var value = CheckExpression(
            statement.Value);

        if (!AreSameType(functionReturnType, value.Type))
        {
            throw new Exception(
                $"Cannot return value of type " +
                $"'{value.Type.GetType().Name}' " +
                $"from function returning " +
                $"'{functionReturnType.GetType().Name}'.");
        }

        return new TypedReturnStatement(
            statement.Statement,
            value);
    }

    private TypedExpression CheckExpression(
        BoundExpression expression)
    {
        return expression switch
        {
            BoundIntegerLiteralExpression integer =>
                new TypedIntegerLiteralExpression(
                    integer.Expression),

            BoundIdentifierExpression identifier =>
                CheckIdentifier(identifier),

            BoundBinaryExpression binary =>
                CheckBinaryExpression(binary),

            _ => throw new Exception(
                $"Unsupported bound expression: " +
                $"{expression.GetType().Name}.")
        };
    }

    private TypedBinaryExpression CheckBinaryExpression(
        BoundBinaryExpression expression)
    {
        var left = CheckExpression(expression.Left);
        var right = CheckExpression(expression.Right);

        if (expression.Expression.OperatorKind is not (TokenKind.Plus or TokenKind.Minus))
        {
            throw new Exception(
                $"Unsupported binary operator: " +
                $"{expression.Expression.OperatorKind}.");
        }

        if (!AreSameType(left.Type, right.Type))
        {
            throw new Exception(
                "Binary operator operands must have the same type.");
        }

        if (left.Type is not IntegerType)
        {
            throw new Exception(
                "The '+' operator currently only supports Integer operands.");
        }

        return new TypedBinaryExpression(
            expression.Expression,
            left,
            right,
            left.Type);
    }

    private TypedIdentifierExpression CheckIdentifier(
        BoundIdentifierExpression expression)
    {
        var variable = expression.Symbol.Declaration;

        if (variable is not VariableDeclaration declaration)
        {
            throw new Exception(
                $"Unsupported identifier declaration: " +
                $"{variable.GetType().Name}.");
        }

        var type = ResolveType(
            declaration.Type);

        return new TypedIdentifierExpression(
            expression.Expression,
            expression.Symbol,
            type);
    }

    private static Type ResolveType(string name)
    {
        return name switch
        {
            "Integer" => new IntegerType(),
            "Float" => new FloatType(),
            "Boolean" => new BooleanType(),
            "String" => new StringType(),
            "Void" => new VoidType(),

            _ => throw new Exception($"Unknown type '{name}'.")
        };
    }

    private static bool AreSameType(Type left, Type right)
    {
        return left.GetType() == right.GetType();
    }
}