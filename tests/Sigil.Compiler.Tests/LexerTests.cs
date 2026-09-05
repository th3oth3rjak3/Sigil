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
}
