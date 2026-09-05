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
}
