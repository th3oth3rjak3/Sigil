using Sigil.Compiler.Syntax;

namespace Sigil.Compiler.Tests;

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
}