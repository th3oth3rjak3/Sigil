using Sigil.Compiler.Semantics;
using Sigil.Compiler.Semantics.BoundDeclarations;
using Sigil.Compiler.Semantics.BoundExpressions;
using Sigil.Compiler.Semantics.BoundPrimitives;
using Sigil.Compiler.Semantics.BoundStatements;
using Sigil.Compiler.Semantics.Primitives;
using Sigil.Compiler.Syntax;
using Sigil.Compiler.Syntax.Declarations;
using Sigil.Compiler.Syntax.Expressions;
using Sigil.Compiler.Syntax.Primitives;
using Sigil.Compiler.Syntax.Statements;

namespace Sigil.Compiler.Tests.Semantics;

public sealed class NameResolverTests
{
    private static Module Parse(string source)
    {
        return new Parser(new Lexer(source)).Parse();
    }

    private static BoundModule Resolve(Module module)
    {
        return new NameResolver(new BuiltinRegistry()).Resolve(module);
    }

    private static BoundModule Resolve(string source)
    {
        return Resolve(Parse(source));
    }

    private static BoundFunctionDeclaration ResolveFunction(
        string source)
    {
        var module = Resolve(source);

        return Assert.Single(module.Declarations);
    }

    [Fact]
    public void ResolvesModule()
    {
        var bound = Resolve("""
            fn main() -> Integer {
                return 42;
            }
            """);

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
        var function = ResolveFunction("""
            fn main() -> Integer {
                return 42;
            }
            """);

        var statement = Assert.IsType<BoundReturnStatement>(
            Assert.Single(function.Body.Statements));

        var expression = Assert.IsType<BoundIntegerLiteralExpression>(
            statement.Value);

        Assert.Equal(42, expression.Expression.Value);
    }

    [Fact]
    public void ResolvesVariableDeclaration()
    {
        var function = ResolveFunction("""
            fn main() -> Integer {
                let x: Integer = 42;
                return x;
            }
            """);

        var let = Assert.IsType<BoundLetStatement>(
            function.Body.Statements[0]);

        Assert.Equal("x", let.Variable.Name);
        Assert.Equal("Integer", let.Variable.Type);
    }

    [Fact]
    public void ResolvesIdentifier()
    {
        var function = ResolveFunction("""
            fn main() -> Integer {
                let x: Integer = 42;
                return x;
            }
            """);

        var returnStatement = Assert.IsType<BoundReturnStatement>(
            function.Body.Statements[1]);

        var identifier = Assert.IsType<BoundIdentifierExpression>(
            returnStatement.Value);

        Assert.Equal("x", identifier.Expression.Name);
    }

    [Fact]
    public void IdentifierResolvesToVariableSymbol()
    {
        var function = ResolveFunction("""
            fn main() -> Integer {
                let x: Integer = 42;
                return x;
            }
            """);

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
        var function = ResolveFunction("""
            fn main() -> Integer {
                let x: Integer = 42;
                let y: Integer = x;
                return x;
            }
            """);

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
        var function = ResolveFunction("""
            fn main() -> Integer {
                let x: Integer = 42;
                let y: Integer = x;
                return y;
            }
            """);

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
        var exception = Assert.Throws<Exception>(
            () => Resolve("""
                fn main() -> Integer {
                    return x;
                }
                """));

        Assert.Contains(
            "The name 'x' could not be resolved",
            exception.Message);
    }

    [Fact]
    public void RejectsSelfReference()
    {
        var exception = Assert.Throws<Exception>(
            () => Resolve("""
                fn main() -> Integer {
                    let x: Integer = x;
                    return x;
                }
                """));

        Assert.Contains(
            "The name 'x' could not be resolved",
            exception.Message);
    }

    [Fact]
    public void RejectsDuplicateVariable()
    {
        var exception = Assert.Throws<Exception>(
            () => Resolve("""
                fn main() -> Integer {
                    let x: Integer = 42;
                    let x: Integer = 43;
                    return x;
                }
                """));

        Assert.Contains(
            "The name 'x' is already declared in this scope",
            exception.Message);
    }

    [Fact]
    public void DifferentVariablesHaveDifferentSymbols()
    {
        var function = ResolveFunction("""
            fn main() -> Integer {
                let x: Integer = 42;
                let y: Integer = 43;
                return x;
            }
            """);

        var x = Assert.IsType<BoundLetStatement>(
            function.Body.Statements[0]);

        var y = Assert.IsType<BoundLetStatement>(
            function.Body.Statements[1]);

        var returnStatement = Assert.IsType<BoundReturnStatement>(
            function.Body.Statements[2]);

        var identifier = Assert.IsType<BoundIdentifierExpression>(
            returnStatement.Value);

        Assert.NotSame(x.Variable, y.Variable);
        Assert.Same(
            x.Variable,
            identifier.Symbol.Declaration);
    }

    [Fact]
    public void DifferentFunctionsHaveIndependentScopes()
    {
        var bound = Resolve("""
            fn first() -> Integer {
                let x: Integer = 42;
                return x;
            }

            fn second() -> Integer {
                let x: Integer = 43;
                return x;
            }
            """);

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
        var function = ResolveFunction("""
            fn main() -> Void {
                return;
            }
            """);

        var statement = Assert.IsType<BoundReturnStatement>(
            Assert.Single(function.Body.Statements));

        Assert.Null(statement.Value);
    }

    [Fact]
    public void EmptyFunctionProducesEmptyBoundBody()
    {
        var function = ResolveFunction("""
            fn main() -> Void {
            }
            """);

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

    [Fact]
    public void ResolvesAdditionExpression()
    {
        var function = ResolveFunction("""
            fn main() -> Integer {
                return 20 + 22;
            }
            """);

        var statement = Assert.IsType<BoundReturnStatement>(
            Assert.Single(function.Body.Statements));

        var expression = Assert.IsType<BoundBinaryExpression>(
            statement.Value);

        Assert.IsType<BoundIntegerLiteralExpression>(
            expression.Left);

        Assert.IsType<BoundIntegerLiteralExpression>(
            expression.Right);

        Assert.Equal(
            TokenKind.Plus,
            expression.Expression.OperatorKind);
    }

    [Fact]
    public void ResolvesMultiplicationExpression()
    {
        var function = ResolveFunction("""
            fn main() -> Integer {
                return 20 * 22;
            }
            """);

        var statement = Assert.IsType<BoundReturnStatement>(
            Assert.Single(function.Body.Statements));

        var expression = Assert.IsType<BoundBinaryExpression>(
            statement.Value);

        Assert.IsType<BoundIntegerLiteralExpression>(
            expression.Left);

        Assert.IsType<BoundIntegerLiteralExpression>(
            expression.Right);

        Assert.Equal(
            TokenKind.Star,
            expression.Expression.OperatorKind);
    }

    [Fact]
    public void ResolvesIntegerDivisionExpression()
    {
        var function = ResolveFunction("""
            fn main() -> Integer {
                return 42 / 2;
            }
            """);

        var statement = Assert.IsType<BoundReturnStatement>(
            Assert.Single(function.Body.Statements));

        var expression = Assert.IsType<BoundBinaryExpression>(
            statement.Value);

        Assert.Equal(
            TokenKind.Slash,
            expression.Expression.OperatorKind);

        Assert.IsType<BoundIntegerLiteralExpression>(expression.Left);
        Assert.IsType<BoundIntegerLiteralExpression>(expression.Right);
    }

    [Fact]
    public void ResolvesFloatDivisionExpression()
    {
        var function = ResolveFunction("""
            fn main() -> Float {
                return 42.0 / 2.0;
            }
            """);

        var statement = Assert.IsType<BoundReturnStatement>(
            Assert.Single(function.Body.Statements));

        var expression = Assert.IsType<BoundBinaryExpression>(
            statement.Value);

        Assert.Equal(
            TokenKind.Slash,
            expression.Expression.OperatorKind);

        Assert.IsType<BoundFloatLiteralExpression>(expression.Left);
        Assert.IsType<BoundFloatLiteralExpression>(expression.Right);
    }

    [Fact]
    public void ResolvesCallExpression()
    {
        var module = Parse("""
        fn foo() -> Integer {
            return 42;
        }

        fn main() -> Integer {
            return foo();
        }
        """);

        var bound = new NameResolver(new BuiltinRegistry()).Resolve(module);

        var main = Assert.IsType<BoundFunctionDeclaration>(
            bound.Declarations[1]);

        var statement = Assert.IsType<BoundReturnStatement>(
            Assert.Single(main.Body.Statements));

        var call = Assert.IsType<BoundCallExpression>(
            statement.Value);

        Assert.Equal("foo", call.Callee.Symbol.Name);
        Assert.Empty(call.Arguments);
    }
}
