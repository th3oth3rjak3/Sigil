using Sigil.Compiler.Semantics;
using Sigil.Compiler.Syntax;

namespace Sigil.Compiler.Tests.Semantics;

public sealed class NameResolverTests
{
    [Fact]
    public void ResolvesModule()
    {
        var module = Parse("""
            fn main() -> Integer {
                return 42;
            }
            """);

        var bound = Resolve(module);

        Assert.Single(bound.Declarations);
    }

    [Fact]
    public void PreservesFunctionDeclaration()
    {
        var module = Parse("""
            fn main() -> Integer {
                return 42;
            }
            """);

        var bound = Resolve(module);

        var function = Assert.Single(bound.Declarations);

        Assert.Same(
            module.Declarations[0],
            function.Declaration);
    }

    [Fact]
    public void ResolvesIntegerLiteral()
    {
        var module = Parse("""
            fn main() -> Integer {
                return 42;
            }
            """);

        var bound = Resolve(module);

        var function = Assert.Single(bound.Declarations);

        var statement = Assert.IsType<BoundReturnStatement>(
            Assert.Single(function.Body.Statements));

        var expression = Assert.IsType<BoundIntegerLiteralExpression>(
            statement.Value);

        Assert.Equal(42, expression.Expression.Value);
    }

    [Fact]
    public void ResolvesVariableDeclaration()
    {
        var module = Parse("""
            fn main() -> Integer {
                let x: Integer = 42;
                return x;
            }
            """);

        var bound = Resolve(module);

        var function = Assert.Single(bound.Declarations);

        var let = Assert.IsType<BoundLetStatement>(
            function.Body.Statements[0]);

        Assert.Equal("x", let.Variable.Name);
        Assert.Equal("Integer", let.Variable.Type);
    }

    [Fact]
    public void ResolvesIdentifier()
    {
        var module = Parse("""
            fn main() -> Integer {
                let x: Integer = 42;
                return x;
            }
            """);

        var bound = Resolve(module);

        var function = Assert.Single(bound.Declarations);

        var returnStatement = Assert.IsType<BoundReturnStatement>(
            function.Body.Statements[1]);

        var identifier = Assert.IsType<BoundIdentifierExpression>(
            returnStatement.Value);

        Assert.Equal("x", identifier.Expression.Name);
    }

    [Fact]
    public void IdentifierResolvesToVariableSymbol()
    {
        var module = Parse("""
            fn main() -> Integer {
                let x: Integer = 42;
                return x;
            }
            """);

        var bound = Resolve(module);

        var function = Assert.Single(bound.Declarations);

        var let = Assert.IsType<BoundLetStatement>(
            function.Body.Statements[0]);

        var returnStatement = Assert.IsType<BoundReturnStatement>(
            function.Body.Statements[1]);

        var identifier = Assert.IsType<BoundIdentifierExpression>(
            returnStatement.Value);

        Assert.Equal("x", identifier.Symbol.Name);
        Assert.Same(
            let.Variable,
            identifier.Symbol.Declaration);
    }

    [Fact]
    public void MultipleReferencesResolveToSameSymbol()
    {
        var module = Parse("""
            fn main() -> Integer {
                let x: Integer = 42;
                let y: Integer = x;
                return x;
            }
            """);

        var bound = Resolve(module);

        var function = Assert.Single(bound.Declarations);

        var x = Assert.IsType<BoundLetStatement>(
            function.Body.Statements[0]);

        var y = Assert.IsType<BoundLetStatement>(
            function.Body.Statements[1]);

        var initializer = Assert.IsType<BoundIdentifierExpression>(
            y.Initializer);

        var returnStatement = Assert.IsType<BoundReturnStatement>(
            function.Body.Statements[2]);

        var returnIdentifier =
            Assert.IsType<BoundIdentifierExpression>(
                returnStatement.Value);

        Assert.Same(
            x.Variable,
            initializer.Symbol.Declaration);

        Assert.Same(
            x.Variable,
            returnIdentifier.Symbol.Declaration);

        Assert.Same(
            initializer.Symbol,
            returnIdentifier.Symbol);
    }

    [Fact]
    public void ResolvesReferenceToEarlierVariable()
    {
        var module = Parse("""
            fn main() -> Integer {
                let x: Integer = 42;
                let y: Integer = x;
                return y;
            }
            """);

        var bound = Resolve(module);

        var function = Assert.Single(bound.Declarations);

        var x = Assert.IsType<BoundLetStatement>(
            function.Body.Statements[0]);

        var y = Assert.IsType<BoundLetStatement>(
            function.Body.Statements[1]);

        var identifier = Assert.IsType<BoundIdentifierExpression>(
            y.Initializer);

        Assert.Same(
            x.Variable,
            identifier.Symbol.Declaration);
    }

    [Fact]
    public void RejectsUnknownIdentifier()
    {
        var module = Parse("""
            fn main() -> Integer {
                return x;
            }
            """);

        var exception = Assert.Throws<Exception>(
            () => Resolve(module));

        Assert.Contains(
            "The name 'x' could not be resolved",
            exception.Message);
    }

    [Fact]
    public void RejectsSelfReference()
    {
        var module = Parse("""
            fn main() -> Integer {
                let x: Integer = x;
                return x;
            }
            """);

        var exception = Assert.Throws<Exception>(
            () => Resolve(module));

        Assert.Contains(
            "The name 'x' could not be resolved",
            exception.Message);
    }

    [Fact]
    public void RejectsDuplicateVariable()
    {
        var module = Parse("""
            fn main() -> Integer {
                let x: Integer = 42;
                let x: Integer = 43;
                return x;
            }
            """);

        var exception = Assert.Throws<Exception>(
            () => Resolve(module));

        Assert.Contains(
            "The name 'x' is already declared in this scope",
            exception.Message);
    }

    [Fact]
    public void DifferentVariablesHaveDifferentSymbols()
    {
        var module = Parse("""
            fn main() -> Integer {
                let x: Integer = 42;
                let y: Integer = 43;
                return x;
            }
            """);

        var bound = Resolve(module);

        var function = Assert.Single(bound.Declarations);

        var x = Assert.IsType<BoundLetStatement>(
            function.Body.Statements[0]);

        var y = Assert.IsType<BoundLetStatement>(
            function.Body.Statements[1]);

        var returnStatement = Assert.IsType<BoundReturnStatement>(
            function.Body.Statements[2]);

        var identifier = Assert.IsType<BoundIdentifierExpression>(
            returnStatement.Value);

        Assert.NotSame(x.Variable, y.Variable);
        Assert.Same(x.Variable, identifier.Symbol.Declaration);
    }

    [Fact]
    public void DifferentFunctionsHaveIndependentScopes()
    {
        var module = Parse("""
            fn first() -> Integer {
                let x: Integer = 42;
                return x;
            }

            fn second() -> Integer {
                let x: Integer = 43;
                return x;
            }
            """);

        var bound = Resolve(module);

        Assert.Equal(2, bound.Declarations.Count);

        var first = bound.Declarations[0];
        var second = bound.Declarations[1];

        var firstLet = Assert.IsType<BoundLetStatement>(
            first.Body.Statements[0]);

        var secondLet = Assert.IsType<BoundLetStatement>(
            second.Body.Statements[0]);

        var firstReturn = Assert.IsType<BoundReturnStatement>(
            first.Body.Statements[1]);

        var secondReturn = Assert.IsType<BoundReturnStatement>(
            second.Body.Statements[1]);

        var firstIdentifier =
            Assert.IsType<BoundIdentifierExpression>(
                firstReturn.Value);

        var secondIdentifier =
            Assert.IsType<BoundIdentifierExpression>(
                secondReturn.Value);

        Assert.Same(
            firstLet.Variable,
            firstIdentifier.Symbol.Declaration);

        Assert.Same(
            secondLet.Variable,
            secondIdentifier.Symbol.Declaration);

        Assert.NotSame(
            firstLet.Variable,
            secondLet.Variable);
    }

    [Fact]
    public void EmptyReturnProducesNoBoundExpression()
    {
        var module = Parse("""
            fn main() -> Void {
                return;
            }
            """);

        var bound = Resolve(module);

        var function = Assert.Single(bound.Declarations);

        var statement = Assert.IsType<BoundReturnStatement>(
            Assert.Single(function.Body.Statements));

        Assert.Null(statement.Value);
    }

    [Fact]
    public void EmptyFunctionProducesEmptyBoundBody()
    {
        var module = Parse("""
            fn main() -> Void {
            }
            """);

        var bound = Resolve(module);

        var function = Assert.Single(bound.Declarations);

        Assert.Empty(function.Body.Statements);
    }

    [Fact]
    public void PreservesOriginalLetStatement()
    {
        var module = Parse("""
            fn main() -> Integer {
                let x: Integer = 42;
                return x;
            }
            """);

        var bound = Resolve(module);

        var function = Assert.Single(bound.Declarations);

        var boundLet = Assert.IsType<BoundLetStatement>(
            function.Body.Statements[0]);

        var syntaxFunction =
            Assert.IsType<FunctionDeclaration>(
                Assert.Single(module.Declarations));

        var syntaxLet =
            Assert.IsType<LetStatement>(
                syntaxFunction.Body.Statements[0]);

        Assert.Same(
            syntaxLet,
            boundLet.Declaration);
    }

    [Fact]
    public void PreservesOriginalIdentifierExpression()
    {
        var module = Parse("""
            fn main() -> Integer {
                let x: Integer = 42;
                return x;
            }
            """);

        var bound = Resolve(module);

        var function = Assert.Single(bound.Declarations);

        var returnStatement = Assert.IsType<BoundReturnStatement>(
            function.Body.Statements[1]);

        var identifier = Assert.IsType<BoundIdentifierExpression>(
            returnStatement.Value);

        var syntaxFunction =
            Assert.IsType<FunctionDeclaration>(
                Assert.Single(module.Declarations));

        var syntaxReturn =
            Assert.IsType<ReturnStatement>(
                syntaxFunction.Body.Statements[1]);

        var syntaxIdentifier =
            Assert.IsType<IdentifierExpression>(
                syntaxReturn.Value);

        Assert.Same(
            syntaxIdentifier,
            identifier.Expression);
    }

    private static Module Parse(string source)
    {
        return new Parser(new Lexer(source)).Parse();
    }

    private static BoundModule Resolve(Module module)
    {
        return new NameResolver().Resolve(module);
    }
}
