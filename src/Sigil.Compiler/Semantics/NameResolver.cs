using Sigil.Compiler.Syntax.Declarations;
using Sigil.Compiler.Syntax.Expressions;
using Sigil.Compiler.Syntax.Primitives;
using Sigil.Compiler.Syntax.Statements;

namespace Sigil.Compiler.Semantics;

public sealed class NameResolver
{
    private readonly BuiltinRegistry _builtins;

    public NameResolver(BuiltinRegistry builtins)
    {
        _builtins = builtins;
    }

    public BoundModule Resolve(Module module)
    {
        var declarations = new List<BoundFunctionDeclaration>(
            module.Declarations.Count);

        var moduleScope = new Scope();

        foreach (var declaration in module.Declarations)
        {
            if (declaration is not FunctionDeclaration function)
            {
                throw new Exception(
                    $"Unsupported declaration: {declaration.GetType().Name}.");
            }

            moduleScope.Declare(
                new Symbol(function.Name, function));
        }

        foreach (var declaration in module.Declarations)
        {
            var function = (FunctionDeclaration)declaration;

            declarations.Add(
                ResolveFunction(function, moduleScope));
        }

        return new BoundModule(declarations);
    }

    private BoundFunctionDeclaration ResolveFunction(
        FunctionDeclaration function,
        Scope parentScope)
    {
        var scope = new Scope(parentScope);

        foreach (var parameter in function.Parameters)
        {
            var declaration = new VariableDeclaration(
                parameter.Name,
                parameter.Type);

            scope.Declare(
                new Symbol(parameter.Name, declaration));
        }

        var body = ResolveBlock(function.Body, scope);

        return new BoundFunctionDeclaration(function, body);
    }

    private BoundBlock ResolveBlock(Block block, Scope scope)
    {
        var statements = new List<BoundStatement>(block.Statements.Count);

        foreach (var statement in block.Statements)
        {
            statements.Add(ResolveStatement(statement, scope));
        }

        return new BoundBlock(statements);
    }

    private BoundStatement ResolveStatement(Statement statement, Scope scope)
    {
        return statement switch
        {
            LetStatement let => ResolveLet(let, scope),
            ReturnStatement @return => ResolveReturn(@return, scope),
            _ => throw new Exception(
                $"Unsupported statement: {statement.GetType().Name}.")
        };
    }

    private BoundLetStatement ResolveLet(
        LetStatement statement,
        Scope scope)
    {
        var variable = new VariableDeclaration(
            statement.Name,
            statement.Type);

        var initializer = ResolveExpression(
            statement.Initializer,
            scope);

        scope.Declare(new Symbol(statement.Name, variable));

        return new BoundLetStatement(
            statement,
            variable,
            initializer);
    }

    private BoundReturnStatement ResolveReturn(
        ReturnStatement statement,
        Scope scope)
    {
        var value = statement.Value is null
            ? null
            : ResolveExpression(statement.Value, scope);

        return new BoundReturnStatement(statement, value);
    }

    private BoundExpression ResolveExpression(
        Expression expression,
        Scope scope)
    {
        return expression switch
        {
            IntegerLiteralExpression integer =>
                new BoundIntegerLiteralExpression(integer),

            FloatLiteralExpression flt =>
                new BoundFloatLiteralExpression(flt),

            IdentifierExpression identifier =>
                ResolveIdentifier(identifier, scope),

            BinaryExpression binary =>
                ResolveBinaryExpression(binary, scope),

            CallExpression call =>
                ResolveCallExpression(call, scope),

            _ => throw new Exception(
                $"Unsupported expression: {expression.GetType().Name}.")
        };
    }

    private BoundIdentifierExpression ResolveIdentifier(
        IdentifierExpression expression,
        Scope scope)
    {
        var symbol = scope.Resolve(expression.Name);

        return new BoundIdentifierExpression(
            expression,
            symbol);
    }

    private BoundBinaryExpression ResolveBinaryExpression(
        BinaryExpression expression,
        Scope scope)
    {
        var left = ResolveExpression(
            expression.Left,
            scope);

        var right = ResolveExpression(
            expression.Right,
            scope);

        return new BoundBinaryExpression(
            expression,
            left,
            right);
    }

    private BoundCallExpression ResolveCallExpression(
        CallExpression expression,
        Scope scope)
    {
        var arguments = expression.Arguments
            .Select(argument => ResolveExpression(argument, scope))
            .ToList();

        var identifier = (IdentifierExpression)expression.Callee;


        var callee = ResolveIdentifier(
            identifier,
            scope);

        return new BoundCallExpression(
            expression,
            callee,
            arguments);
    }
}
