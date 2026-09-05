using Sigil.Compiler.Syntax;

namespace Sigil.Compiler.Tests.Syntax;

public class ParserTests
{
    private static Module Parse(string source)
    {
        var lexer = new Lexer(source);
        var parser = new Parser(lexer);

        return parser.Parse();
    }

    [Fact]
    public void ParsesFunctionDeclaration()
    {
        var module = Parse("fn main() -> Void {}");

        Assert.Single(module.Declarations);

        var function = Assert.IsType<FunctionDeclaration>(
            module.Declarations[0]);

        Assert.Equal("main", function.Name);
        Assert.Empty(function.Parameters);
        Assert.Empty(function.Body.Statements);
    }

    [Fact]
    public void RejectsFunctionWithoutName()
    {
        Assert.Throws<Exception>(
            () => Parse("fn () {}"));
    }

    [Fact]
    public void RejectsFunctionWithoutClosingBrace()
    {
        Assert.Throws<Exception>(
            () => Parse("fn main() {"));
    }

    [Fact]
    public void RejectsFunctionWithoutReturnType()
    {
        Assert.Throws<Exception>(
            () => Parse("fn main() {}"));
    }

    [Fact]
    public void ParsesReturnStatement()
    {
        var module = Parse("""
            fn main() -> Integer {
                return 42;
            }
            """);

        var function = Assert.IsType<FunctionDeclaration>(
            module.Declarations[0]);

        Assert.Single(function.Body.Statements);
        Assert.Equal("Integer", function.ReturnType);

        var statement = Assert.IsType<ReturnStatement>(
            function.Body.Statements[0]);

        var expression = Assert.IsType<IntegerLiteralExpression>(
            statement.Value);

        Assert.Equal(42, expression.Value);
    }

    [Fact]
    public void ParsesEmptyReturnStatement()
    {
        var module = Parse("""
            fn main() -> Void {
                return;
            }
            """);

        var function = Assert.IsType<FunctionDeclaration>(
            module.Declarations[0]);

        var statement = Assert.IsType<ReturnStatement>(
            Assert.Single(function.Body.Statements));

        Assert.Equal("Void", function.ReturnType);
        Assert.Null(statement.Value);
    }

    [Fact]
    public void ParsesFunctionParameters()
    {
        var module = Parse("""
            fn add(a: Integer, b: Integer) -> Integer {
                return 42;
            }
            """);

        var function = Assert.IsType<FunctionDeclaration>(
            module.Declarations[0]);

        Assert.Equal(
            [new Parameter("a", "Integer"), new Parameter("b", "Integer")],
            function.Parameters);
    }

    [Fact]
    public void RejectsParameterWithoutName()
    {
        Assert.Throws<Exception>(
            () => Parse("""
                fn add(: Integer) -> Integer {
                }
                """));
    }

    [Fact]
    public void RejectsParameterWithoutType()
    {
        Assert.Throws<Exception>(
            () => Parse("""
                fn add(a:) -> Integer {
                }
                """));
    }

    [Fact]
    public void RejectsParametersWithoutComma()
    {
        Assert.Throws<Exception>(
            () => Parse("""
                fn add(a: Integer b: Integer) -> Integer {
                }
                """));
    }

    [Fact]
    public void ParsesLetStatement()
    {
        var module = Parse("""
            fn main() -> Integer {
                let x: Integer = 42;
                return x;
            }
            """);

        var function = Assert.IsType<FunctionDeclaration>(
            Assert.Single(module.Declarations));

        Assert.Equal(2, function.Body.Statements.Count);

        var let = Assert.IsType<LetStatement>(
            function.Body.Statements[0]);

        Assert.Equal("x", let.Name);
        Assert.Equal("Integer", let.Type);

        var initializer = Assert.IsType<IntegerLiteralExpression>(
            let.Initializer);

        Assert.Equal(42, initializer.Value);

        var returnStatement = Assert.IsType<ReturnStatement>(
            function.Body.Statements[1]);

        var identifier = Assert.IsType<IdentifierExpression>(
            returnStatement.Value);

        Assert.Equal("x", identifier.Name);
    }

    [Fact]
    public void ParsesAdditionExpression()
    {
        var module = Parse("""
            fn main() -> Integer {
                return 20 + 22;
            }
            """);

        var function = Assert.IsType<FunctionDeclaration>(
            Assert.Single(module.Declarations));

        var statement = Assert.IsType<ReturnStatement>(
            Assert.Single(function.Body.Statements));

        var expression = Assert.IsType<BinaryExpression>(
            statement.Value);

        Assert.Equal(TokenKind.Plus, expression.OperatorKind);

        var left = Assert.IsType<IntegerLiteralExpression>(
            expression.Left);

        var right = Assert.IsType<IntegerLiteralExpression>(
            expression.Right);

        Assert.Equal(20, left.Value);
        Assert.Equal(22, right.Value);
    }

    [Fact]
    public void ParsesMultiplicationExpression()
    {
        var module = Parse("""
            fn main() -> Integer {
                return 20 * 22;
            }
            """);

        var function = Assert.IsType<FunctionDeclaration>(
            Assert.Single(module.Declarations));

        var statement = Assert.IsType<ReturnStatement>(
            Assert.Single(function.Body.Statements));

        var expression = Assert.IsType<BinaryExpression>(
            statement.Value);

        Assert.Equal(TokenKind.Star, expression.OperatorKind);

        var left = Assert.IsType<IntegerLiteralExpression>(
            expression.Left);

        var right = Assert.IsType<IntegerLiteralExpression>(
            expression.Right);

        Assert.Equal(20, left.Value);
        Assert.Equal(22, right.Value);
    }

    [Fact]
    public void ParsesIntegerDivisionExpression()
    {
        var module = Parse("""
            fn main() -> Integer {
                return 42 / 2;
            }
            """);

        var function = Assert.IsType<FunctionDeclaration>(
            Assert.Single(module.Declarations));

        var statement = Assert.IsType<ReturnStatement>(
            Assert.Single(function.Body.Statements));

        var expression = Assert.IsType<BinaryExpression>(
            statement.Value);

        Assert.Equal(TokenKind.Slash, expression.OperatorKind);

        Assert.IsType<IntegerLiteralExpression>(expression.Left);
        Assert.IsType<IntegerLiteralExpression>(expression.Right);
    }

    [Fact]
    public void ParsesFloatDivisionExpression()
    {
        var module = Parse("""
            fn main() -> Float {
                return 42.0 / 2.0;
            }
            """);

        var function = Assert.IsType<FunctionDeclaration>(
            Assert.Single(module.Declarations));

        var statement = Assert.IsType<ReturnStatement>(
            Assert.Single(function.Body.Statements));

        var expression = Assert.IsType<BinaryExpression>(
            statement.Value);

        Assert.Equal(TokenKind.Slash, expression.OperatorKind);

        Assert.IsType<FloatLiteralExpression>(expression.Left);
        Assert.IsType<FloatLiteralExpression>(expression.Right);
    }

    [Fact]
    public void ParsesCallExpression()
    {
        var module = Parse("""
        fn main() -> Integer {
            return println(42);
        }
        """);

        var function = Assert.IsType<FunctionDeclaration>(
            Assert.Single(module.Declarations));

        var statement = Assert.IsType<ReturnStatement>(
            Assert.Single(function.Body.Statements));

        var call = Assert.IsType<CallExpression>(
            statement.Value);

        var callee = Assert.IsType<IdentifierExpression>(
            call.Callee);

        Assert.Equal("println", callee.Name);

        var argument = Assert.Single(call.Arguments);

        var literal = Assert.IsType<IntegerLiteralExpression>(
            argument);

        Assert.Equal(42, literal.Value);
    }

    [Fact]
    public void ParsesCallExpressionWithMultipleArguments()
    {
        var module = Parse("""
        fn main() -> Integer {
            return foo(1, 2, 3);
        }
        """);

        var function = Assert.IsType<FunctionDeclaration>(
            Assert.Single(module.Declarations));

        var statement = Assert.IsType<ReturnStatement>(
            Assert.Single(function.Body.Statements));

        var call = Assert.IsType<CallExpression>(statement.Value);

        Assert.Equal("foo", Assert.IsType<IdentifierExpression>(call.Callee).Name);
        Assert.Equal(3, call.Arguments.Count);
    }

    [Fact]
    public void ParsesCallExpressionWithNoArguments()
    {
        var module = Parse("""
        fn main() -> Integer {
            return foo();
        }
        """);

        var function = Assert.IsType<FunctionDeclaration>(
            Assert.Single(module.Declarations));

        var statement = Assert.IsType<ReturnStatement>(
            Assert.Single(function.Body.Statements));

        var call = Assert.IsType<CallExpression>(statement.Value);

        Assert.Equal("foo", Assert.IsType<IdentifierExpression>(call.Callee).Name);
        Assert.Empty(call.Arguments);
    }
}
