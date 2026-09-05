using Sigil.Compiler.Syntax;

namespace Sigil.Compiler.Tests;

public class LexerTests
{
    [Fact]
    public void LexesLetAndIdentifier()
    {
        var lexer = new Lexer("let hello");

        Assert.Equal(new Token(TokenKind.Let, "let", 0, 3), lexer.NextToken());
        Assert.Equal(new Token(TokenKind.Identifier, "hello", 4, 5), lexer.NextToken());
        Assert.Equal(new Token(TokenKind.EndOfFile, "", 9, 0), lexer.NextToken());
    }

    [Fact]
    public void LexesSemicolon()
    {
        var lexer = new Lexer("let x;");

        Assert.Equal(new Token(TokenKind.Let, "let", 0, 3), lexer.NextToken());
        Assert.Equal(new Token(TokenKind.Identifier, "x", 4, 1), lexer.NextToken());
        Assert.Equal(new Token(TokenKind.Semicolon, ";", 5, 1), lexer.NextToken());
        Assert.Equal(new Token(TokenKind.EndOfFile, "", 6, 0), lexer.NextToken());
    }

    [Fact]
    public void LexesLeftParen()
    {
        var lexer = new Lexer("(");

        Assert.Equal(new Token(TokenKind.LeftParen, "(", 0, 1), lexer.NextToken());
        Assert.Equal(new Token(TokenKind.EndOfFile, "", 1, 0), lexer.NextToken());
    }

    [Fact]
    public void LexesRightParen()
    {
        var lexer = new Lexer(")");

        Assert.Equal(new Token(TokenKind.RightParen, ")", 0, 1), lexer.NextToken());
        Assert.Equal(new Token(TokenKind.EndOfFile, "", 1, 0), lexer.NextToken());
    }

    [Fact]
    public void LexesLeftBrace()
    {
        var lexer = new Lexer("{");

        Assert.Equal(new Token(TokenKind.LeftBrace, "{", 0, 1), lexer.NextToken());
        Assert.Equal(new Token(TokenKind.EndOfFile, "", 1, 0), lexer.NextToken());
    }

    [Fact]
    public void LexesRightBrace()
    {
        var lexer = new Lexer("}");

        Assert.Equal(new Token(TokenKind.RightBrace, "}", 0, 1), lexer.NextToken());
        Assert.Equal(new Token(TokenKind.EndOfFile, "", 1, 0), lexer.NextToken());
    }

    [Fact]
    public void LexesComma()
    {
        var lexer = new Lexer(",");

        Assert.Equal(new Token(TokenKind.Comma, ",", 0, 1), lexer.NextToken());
        Assert.Equal(new Token(TokenKind.EndOfFile, "", 1, 0), lexer.NextToken());
    }


    [Fact]
    public void LexesColon()
    {
        var lexer = new Lexer(":");

        Assert.Equal(new Token(TokenKind.Colon, ":", 0, 1), lexer.NextToken());
        Assert.Equal(new Token(TokenKind.EndOfFile, "", 1, 0), lexer.NextToken());
    }

    [Fact]
    public void LexesIntegerLiteral()
    {
        var lexer = new Lexer("42");

        Assert.Equal(new Token(TokenKind.IntegerLiteral, "42", 0, 2), lexer.NextToken());
        Assert.Equal(new Token(TokenKind.EndOfFile, "", 2, 0), lexer.NextToken());
    }

    [Fact]
    public void LexesFnKeyword()
    {
        var lexer = new Lexer("fn");

        Assert.Equal(new Token(TokenKind.Fn, "fn", 0, 2), lexer.NextToken());
        Assert.Equal(new Token(TokenKind.EndOfFile, "", 2, 0), lexer.NextToken());
    }
}
