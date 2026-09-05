using Sigil.Compiler.Syntax;

namespace Sigil.Compiler.Tests.Syntax;

public class ParserTests
{
    [Fact]
    public void ParsesFunctionDeclaration()
    {
        var parser = new Parser(new Lexer("fn main() -> Void {}"));

        var module = parser.Parse();

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
        var parser = new Parser(new Lexer("fn () {}"));

        Assert.Throws<Exception>(parser.Parse);
    }

    [Fact]
    public void RejectsFunctionWithoutClosingBrace()
    {
        var parser = new Parser(new Lexer("fn main() {"));

        Assert.Throws<Exception>(parser.Parse);
    }

    [Fact]
    public void RejectsFunctionWithoutReturnType()
    {
        var parser = new Parser(new Lexer("fn main() {}"));
        Assert.Throws<Exception>(parser.Parse);
    }

    [Fact]
    public void ParsesReturnStatement()
    {
        var parser = new Parser(new Lexer("""
        fn main() -> Integer {
            return 42;
        }
        """));

        var module = parser.Parse();

        var function = Assert.IsType<FunctionDeclaration>(
            module.Declarations[0]);

        Assert.Single(function.Body.Statements);
        Assert.Equal("Integer", function.ReturnType);
        var statement = Assert.IsType<ReturnStatement>(function.Body.Statements[0]);
        var expression = Assert.IsType<IntegerLiteralExpression>(statement.Value);
        Assert.Equal(42, expression.Value);
    }

    [Fact]
    public void ParsesEmptyReturnStatement()
    {
        var parser = new Parser(new Lexer("""
        fn main() -> Void {
            return;
        }
        """));

        var module = parser.Parse();

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
        var parser = new Parser(new Lexer("""
        fn add(a: Integer, b: Integer) -> Integer {
            return 42;
        }
        """));

        var module = parser.Parse();

        var function = Assert.IsType<FunctionDeclaration>(
            module.Declarations[0]);

        Assert.Equal([new Parameter("a", "Integer"), new Parameter("b", "Integer")], function.Parameters);
    }

    [Fact]
    public void RejectsParameterWithoutName()
    {
        var parser = new Parser(new Lexer("""
        fn add(: Integer) -> Integer {
        }
        """));

        Assert.Throws<Exception>(parser.Parse);
    }

    [Fact]
    public void RejectsParameterWithoutType()
    {
        var parser = new Parser(new Lexer("""
        fn add(a:) -> Integer {
        }
        """));

        Assert.Throws<Exception>(parser.Parse);
    }

    [Fact]
    public void RejectsParametersWithoutComma()
    {
        var parser = new Parser(new Lexer("""
        fn add(a: Integer b: Integer) -> Integer {
        }
        """));

        Assert.Throws<Exception>(parser.Parse);
    }

    [Fact]
    public void ParsesLetStatement()
    {
        var source = """
        fn main() -> Integer {
            let x: Integer = 42;
            return x;
        }
        """;

        var parser = new Parser(new Lexer(source));

        var module = parser.Parse();

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
        var module = new Parser(new Lexer("""
        fn main() -> Integer {
            return 20 + 22;
        }
        """)).Parse();

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
        var module = new Parser(new Lexer(
        """
              fn main() -> Integer {
                  return 20 * 22;
              }
              """)).Parse();

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
}
