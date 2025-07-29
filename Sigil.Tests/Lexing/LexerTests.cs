
using Sigil.ErrorHandling;
using Sigil.Lexing;

namespace Sigil.Tests.Lexing;

public class LexerTests
{
    [Theory]
    [InlineData("(", TokenType.LeftParen)]
    [InlineData(")", TokenType.RightParen)]
    [InlineData("{", TokenType.LeftBrace)]
    [InlineData("}", TokenType.RightBrace)]
    [InlineData("[", TokenType.LeftBracket)]
    [InlineData("]", TokenType.RightBracket)]
    [InlineData(",", TokenType.Comma)]
    [InlineData(";", TokenType.Semicolon)]
    [InlineData(":", TokenType.Colon)]
    [InlineData(".", TokenType.Dot)]
    public void TheLexer_ShouldLexDelimiterTokens_WhenValid(string input, TokenType expected)
    {
        var errorHandler = new ErrorHandler(input);
        var lexer = new Lexer(input, errorHandler);

        var tokens = lexer.Tokenize();
        Assert.Equal(2, tokens.Count);
        Assert.Equal(expected, tokens.First().TokenType);
        Assert.Equal(TokenType.Eof, tokens.Last().TokenType);
    }

    [Theory]
    [InlineData("123", TokenType.IntegerLiteral)]
    [InlineData("3.14", TokenType.FloatLiteral)]
    [InlineData("\"hello\"", TokenType.StringLiteral)]
    public void TheLexer_ShouldLexLiteralTokens_WhenValid(string input, TokenType expected)
    {
        var errorHandler = new ErrorHandler(input);
        var lexer = new Lexer(input, errorHandler);

        var tokens = lexer.Tokenize();
        Assert.Equal(2, tokens.Count);
        Assert.Equal(expected, tokens.First().TokenType);
        Assert.Equal(TokenType.Eof, tokens.Last().TokenType);
    }

    [Theory]
    [InlineData("+", TokenType.Plus)]
    [InlineData("+=", TokenType.PlusEqual)]
    [InlineData("-", TokenType.Minus)]
    [InlineData("-=", TokenType.MinusEqual)]
    [InlineData("*", TokenType.Star)]
    [InlineData("*=", TokenType.StarEqual)]
    [InlineData("/", TokenType.Slash)]
    [InlineData("/=", TokenType.SlashEqual)]
    [InlineData("=", TokenType.Equal)]
    [InlineData("==", TokenType.EqualEqual)]
    [InlineData("!", TokenType.Bang)]
    [InlineData("!=", TokenType.BangEqual)]
    [InlineData("<", TokenType.Less)]
    [InlineData("<=", TokenType.LessEqual)]
    [InlineData(">", TokenType.Greater)]
    [InlineData(">=", TokenType.GreaterEqual)]
    [InlineData("->", TokenType.Arrow)]
    [InlineData("=>", TokenType.FatArrow)]
    public void TheLexer_ShouldLexOperatorTokens_WhenValid(string input, TokenType expected)
    {
        var errorHandler = new ErrorHandler(input);
        var lexer = new Lexer(input, errorHandler);

        var tokens = lexer.Tokenize();
        Assert.Equal(2, tokens.Count);
        Assert.Equal(expected, tokens.First().TokenType);
        Assert.Equal(TokenType.Eof, tokens.Last().TokenType);
    }

    [Theory]
    [InlineData("       // a comment goes here")]
    [InlineData("// a comment here                ")]
    [InlineData("// just a comment")]
    [InlineData("           ")] // tabs
    public void TheLexer_ShouldSkipWhitespaceAndComments(string input)
    {
        var errorHandler = new ErrorHandler(input);
        var lexer = new Lexer(input, errorHandler);

        var tokens = lexer.Tokenize();
        Assert.Single(tokens);
        Assert.Equal(TokenType.Eof, tokens.Single().TokenType);
    }

    [Fact]
    public void TheLexer_ShouldHandleLargerLexingTasks()
    {
        var source = """
// This comment should be ignored.
fun add(a, b) {
    return a + b;
}

let valid_identifier_1 = "something";
""";

        var errorHandler = new ErrorHandler(source);
        var lexer = new Lexer(source, errorHandler);

        var tokens = lexer.Tokenize();

        List<TokenType> expected = [
            TokenType.Fun,
            TokenType.Identifier, // add
            TokenType.LeftParen,
            TokenType.Identifier, // a
            TokenType.Comma,
            TokenType.Identifier, // b
            TokenType.RightParen,
            TokenType.LeftBrace,
            TokenType.Return,
            TokenType.Identifier, // a
            TokenType.Plus,
            TokenType.Identifier, // b
            TokenType.Semicolon,
            TokenType.RightBrace,
            TokenType.Let,
            TokenType.Identifier,
            TokenType.Equal,
            TokenType.StringLiteral,
            TokenType.Semicolon,
            TokenType.Eof,
        ];

        Assert.Equal(expected.Count, tokens.Count);

        for (var i = 0; i < tokens.Count; i++)
        {
            Assert.Equal(expected[i], tokens[i].TokenType);
        }
    }

    [Theory]
    [InlineData("abc_123", 6)]
    public void TheLexer_ShouldProduceValidIdentifierSpans(string input, int expectedEndOffset)
    {
        var errorHandler = new ErrorHandler(input);
        var lexer = new Lexer(input, errorHandler);

        var tokens = lexer.Tokenize();

        Assert.Equal(2, tokens.Count);

        var testToken = tokens.First();
        var eofToken = tokens.Last();

        Assert.Equal(TokenType.Identifier, testToken.TokenType);
        Assert.Equal(expectedEndOffset, testToken.Span.End.Offset);

        Assert.Equal("abc_123", testToken.Span.Slice(input));

        Assert.Equal(TokenType.Eof, eofToken.TokenType);
    }
}
