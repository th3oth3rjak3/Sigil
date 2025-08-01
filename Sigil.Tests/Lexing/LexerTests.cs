
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

    [Theory]
    [InlineData("break", TokenType.Break)]
    [InlineData("class", TokenType.Class)]
    [InlineData("else", TokenType.Else)]
    [InlineData("false", TokenType.False)]
    [InlineData("for", TokenType.For)]
    [InlineData("fun", TokenType.Fun)]
    [InlineData("if", TokenType.If)]
    [InlineData("let", TokenType.Let)]
    [InlineData("new", TokenType.New)]
    [InlineData("this", TokenType.This)]
    [InlineData("while", TokenType.While)]
    [InlineData("return", TokenType.Return)]
    [InlineData("true", TokenType.True)]
    [InlineData("continue", TokenType.Continue)]
    [InlineData("or", TokenType.Or)]
    [InlineData("and", TokenType.And)]
    public void TheLexer_ShouldLexKeywordTokens_WhenValid(string input, TokenType expected)
    {
        var errorHandler = new ErrorHandler(input);
        var lexer = new Lexer(input, errorHandler);
        var tokens = lexer.Tokenize();
        Assert.Equal(2, tokens.Count);
        Assert.Equal(expected, tokens.First().TokenType);
        Assert.Equal(TokenType.Eof, tokens.Last().TokenType);
    }

    [Theory]
    [InlineData("identifier")]
    [InlineData("variable_name")]
    [InlineData("camelCase")]
    [InlineData("snake_case")]
    [InlineData("identifier123")]
    [InlineData("a")]
    [InlineData("test_123_abc")]
    public void TheLexer_ShouldLexIdentifierTokens_WhenValid(string input)
    {
        var errorHandler = new ErrorHandler(input);
        var lexer = new Lexer(input, errorHandler);
        var tokens = lexer.Tokenize();
        Assert.Equal(2, tokens.Count);
        Assert.Equal(TokenType.Identifier, tokens.First().TokenType);
        Assert.Equal(TokenType.Eof, tokens.Last().TokenType);
        Assert.Equal(input, tokens.First().Span.Slice(input));
    }

    [Theory]
    [InlineData("0")]
    [InlineData("42")]
    [InlineData("123456789")]
    [InlineData("007")]
    public void TheLexer_ShouldLexIntegerLiterals_WhenValid(string input)
    {
        var errorHandler = new ErrorHandler(input);
        var lexer = new Lexer(input, errorHandler);
        var tokens = lexer.Tokenize();
        Assert.Equal(2, tokens.Count);
        Assert.Equal(TokenType.IntegerLiteral, tokens.First().TokenType);
        Assert.Equal(TokenType.Eof, tokens.Last().TokenType);
        Assert.Equal(input, tokens.First().Span.Slice(input));
    }

    [Theory]
    [InlineData("3.14")]
    [InlineData("0.5")]
    [InlineData("123.456")]
    [InlineData("0.0")]
    [InlineData("999.999")]
    public void TheLexer_ShouldLexFloatLiterals_WhenValid(string input)
    {
        var errorHandler = new ErrorHandler(input);
        var lexer = new Lexer(input, errorHandler);
        var tokens = lexer.Tokenize();
        Assert.Equal(2, tokens.Count);
        Assert.Equal(TokenType.FloatLiteral, tokens.First().TokenType);
        Assert.Equal(TokenType.Eof, tokens.Last().TokenType);
        Assert.Equal(input, tokens.First().Span.Slice(input));
    }

    [Theory]
    [InlineData("\"\"")]
    [InlineData("\"hello\"")]
    [InlineData("\"hello world\"")]
    [InlineData("\"with spaces and 123 numbers\"")]
    [InlineData("\"special!@#$%^&*()characters\"")]
    public void TheLexer_ShouldLexStringLiterals_WhenValid(string input)
    {
        var errorHandler = new ErrorHandler(input);
        var lexer = new Lexer(input, errorHandler);
        var tokens = lexer.Tokenize();
        Assert.Equal(2, tokens.Count);
        Assert.Equal(TokenType.StringLiteral, tokens.First().TokenType);
        Assert.Equal(TokenType.Eof, tokens.Last().TokenType);
        Assert.Equal(input, tokens.First().Span.Slice(input));
    }

    [Fact]
    public void TheLexer_ShouldHandleUnterminatedString_AndReportError()
    {
        var input = "\"unterminated string";
        var errorHandler = new ErrorHandler(input);
        var lexer = new Lexer(input, errorHandler);
        var tokens = lexer.Tokenize();

        Assert.Equal(2, tokens.Count);
        Assert.Equal(TokenType.Invalid, tokens.First().TokenType);
        Assert.Equal(TokenType.Eof, tokens.Last().TokenType);
        Assert.True(errorHandler.HadError);
    }

    [Theory]
    [InlineData("/// This is a docstring comment")]
    [InlineData("/// Multi-line\n/// docstring\n/// comment")]
    [InlineData("/// Single line with special chars !@#$%")]
    public void TheLexer_ShouldLexDocstringComments_WhenValid(string input)
    {
        var errorHandler = new ErrorHandler(input);
        var lexer = new Lexer(input, errorHandler);
        var tokens = lexer.Tokenize();
        Assert.Equal(2, tokens.Count);
        Assert.Equal(TokenType.DocStringComment, tokens.First().TokenType);
        Assert.Equal(TokenType.Eof, tokens.Last().TokenType);
    }

    [Theory]
    [InlineData("@")]
    [InlineData("#")]
    [InlineData("$")]
    [InlineData("%")]
    [InlineData("^")]
    [InlineData("&")]
    [InlineData("`")]
    [InlineData("~")]
    public void TheLexer_ShouldProduceInvalidTokens_ForUnrecognizedCharacters(string input)
    {
        var errorHandler = new ErrorHandler(input);
        var lexer = new Lexer(input, errorHandler);
        var tokens = lexer.Tokenize();

        Assert.Equal(2, tokens.Count);
        Assert.Equal(TokenType.Invalid, tokens.First().TokenType);
        Assert.Equal(TokenType.Eof, tokens.Last().TokenType);
        Assert.True(errorHandler.HadError);
    }

    [Fact]
    public void TheLexer_ShouldHandleEmptyInput()
    {
        var input = "";
        var errorHandler = new ErrorHandler(input);
        var lexer = new Lexer(input, errorHandler);
        var tokens = lexer.Tokenize();

        Assert.Single(tokens);
        Assert.Equal(TokenType.Eof, tokens.First().TokenType);
    }

    [Theory]
    [InlineData(" \t\n\r")]
    [InlineData("   ")]
    [InlineData("\n\n\n")]
    [InlineData("\t\t\t")]
    public void TheLexer_ShouldSkipWhitespace_AndOnlyProduceEof(string input)
    {
        var errorHandler = new ErrorHandler(input);
        var lexer = new Lexer(input, errorHandler);
        var tokens = lexer.Tokenize();

        Assert.Single(tokens);
        Assert.Equal(TokenType.Eof, tokens.First().TokenType);
    }

    [Fact]
    public void TheLexer_ShouldDistinguishBetweenBasicAndDocstringComments()
    {
        var input = "// basic comment\n/// docstring comment";
        var errorHandler = new ErrorHandler(input);
        var lexer = new Lexer(input, errorHandler);
        var tokens = lexer.Tokenize();

        Assert.Equal(2, tokens.Count);
        Assert.Equal(TokenType.DocStringComment, tokens.First().TokenType);
        Assert.Equal(TokenType.Eof, tokens.Last().TokenType);
    }

    [Theory]
    [InlineData("123.")]
    [InlineData("123.abc")]
    [InlineData("123.   ")]
    public void TheLexer_ShouldNotLexInvalidFloats_AsFloatLiterals(string input)
    {
        var errorHandler = new ErrorHandler(input);
        var lexer = new Lexer(input, errorHandler);
        var tokens = lexer.Tokenize();

        // Should produce integer + dot + whatever follows
        Assert.True(tokens.Count >= 3); // at least integer, dot, eof
        Assert.Equal(TokenType.IntegerLiteral, tokens[0].TokenType);
        Assert.Equal(TokenType.Dot, tokens[1].TokenType);
    }

    [Fact]
    public void TheLexer_ShouldHandleComplexExpression()
    {
        var input = "let result = (a + b) * 3.14;";
        var errorHandler = new ErrorHandler(input);
        var lexer = new Lexer(input, errorHandler);
        var tokens = lexer.Tokenize();

        List<TokenType> expected = [
            TokenType.Let,
            TokenType.Identifier, // result
            TokenType.Equal,
            TokenType.LeftParen,
            TokenType.Identifier, // a
            TokenType.Plus,
            TokenType.Identifier, // b
            TokenType.RightParen,
            TokenType.Star,
            TokenType.FloatLiteral, // 3.14
            TokenType.Semicolon,
            TokenType.Eof
        ];

        Assert.Equal(expected.Count, tokens.Count);
        for (var i = 0; i < tokens.Count; i++)
        {
            Assert.Equal(expected[i], tokens[i].TokenType);
        }
    }

    [Fact]
    public void TheLexer_ShouldHandleMultiLineDocstringComments()
    {
        var input = """
/// a comment
/// that continues
/// On multiple lines
""";
        var errorHandler = new ErrorHandler(input);
        var lexer = new Lexer(input, errorHandler);
        var tokens = lexer.Tokenize();

        Assert.Equal(2, tokens.Count);
        Assert.Equal(TokenType.DocStringComment, tokens.First().TokenType);
        Assert.Equal(TokenType.Eof, tokens.Last().TokenType);

        // Verify the span covers all three lines
        var docstringToken = tokens.First();
        var slicedText = docstringToken.Span.Slice(input);
        Assert.Contains("a comment", slicedText);
        Assert.Contains("that continues", slicedText);
        Assert.Contains("On multiple lines", slicedText);
    }

    [Fact]
    public void TheLexer_ShouldHandleDocstringCommentsFollowedByCode()
    {
        var input = """
/// This is a docstring
/// for a function
fun test() {}
""";
        var errorHandler = new ErrorHandler(input);
        var lexer = new Lexer(input, errorHandler);
        var tokens = lexer.Tokenize();

        Assert.Equal(8, tokens.Count);
        Assert.Equal(TokenType.DocStringComment, tokens[0].TokenType);
        Assert.Equal(TokenType.Fun, tokens[1].TokenType);
        Assert.Equal(TokenType.Identifier, tokens[2].TokenType);
        Assert.Equal(TokenType.LeftParen, tokens[3].TokenType);
        Assert.Equal(TokenType.RightParen, tokens[4].TokenType);
        Assert.Equal(TokenType.LeftBrace, tokens[5].TokenType);
        Assert.Equal(TokenType.RightBrace, tokens[6].TokenType);
        Assert.Equal(TokenType.Eof, tokens[7].TokenType);
    }

    [Theory]
    [InlineData("1", 1, 1, 0)]
    [InlineData("abc", 1, 3, 2)]
    [InlineData("\"hello " + "\n" + "world\"", 2, 6, 13)]
    public void TheLexer_ShouldTrackPositionCorrectly(string input, int expectedEndLine, int expectedEndColumn, int expectedEndOffset)
    {
        var errorHandler = new ErrorHandler(input);
        var lexer = new Lexer(input, errorHandler);
        var tokens = lexer.Tokenize();

        var firstToken = tokens.First();
        Assert.Equal(1, firstToken.Span.Start.Line);
        Assert.Equal(1, firstToken.Span.Start.Column);
        Assert.Equal(0, firstToken.Span.Start.Offset);
        Assert.Equal(expectedEndLine, firstToken.Span.End.Line);
        Assert.Equal(expectedEndColumn, firstToken.Span.End.Column);
        Assert.Equal(expectedEndOffset, firstToken.Span.End.Offset);
    }

    [Theory]
    [InlineData("//")]
    [InlineData("// ")]
    [InlineData("//comment")]
    [InlineData("// comment with spaces")]
    public void TheLexer_ShouldSkipBasicComments_WithoutNewline(string input)
    {
        var errorHandler = new ErrorHandler(input);
        var lexer = new Lexer(input, errorHandler);
        var tokens = lexer.Tokenize();

        Assert.Single(tokens);
        Assert.Equal(TokenType.Eof, tokens.First().TokenType);
    }

    [Fact]
    public void TheLexer_ShouldHandleMixedCommentsAndWhitespace()
    {
        var input = """
  // first comment

  // second comment

/// docstring
let x = 5;
""";
        var errorHandler = new ErrorHandler(input);
        var lexer = new Lexer(input, errorHandler);
        var tokens = lexer.Tokenize();

        Assert.Equal(7, tokens.Count);
        Assert.Equal(TokenType.DocStringComment, tokens[0].TokenType);
        Assert.Equal(TokenType.Let, tokens[1].TokenType);
        Assert.Equal(TokenType.Identifier, tokens[2].TokenType);
        Assert.Equal(TokenType.Equal, tokens[3].TokenType);
        Assert.Equal(TokenType.IntegerLiteral, tokens[4].TokenType);
        Assert.Equal(TokenType.Semicolon, tokens[5].TokenType);
        Assert.Equal(TokenType.Eof, tokens[6].TokenType);
    }

    [Theory]
    [InlineData("123abc", TokenType.IntegerLiteral, TokenType.Identifier)]
    [InlineData("3.14abc", TokenType.FloatLiteral, TokenType.Identifier)]
    [InlineData("letx", TokenType.Identifier)] // Should be one identifier, not "let" + "x"
    public void TheLexer_ShouldHandleTokenBoundaries_Correctly(string input, params TokenType[] expectedTypes)
    {
        var errorHandler = new ErrorHandler(input);
        var lexer = new Lexer(input, errorHandler);
        var tokens = lexer.Tokenize();

        // Remove EOF token for comparison
        var actualTypes = tokens.Take(tokens.Count - 1).Select(t => t.TokenType).ToArray();
        Assert.Equal(expectedTypes, actualTypes);
    }

    [Fact]
    public void TheLexer_ShouldHandleConsecutiveOperators()
    {
        var input = "+-*/";
        var errorHandler = new ErrorHandler(input);
        var lexer = new Lexer(input, errorHandler);
        var tokens = lexer.Tokenize();

        List<TokenType> expected = [
            TokenType.Plus,
            TokenType.Minus,
            TokenType.Star,
            TokenType.Slash,
            TokenType.Eof
        ];

        Assert.Equal(expected.Count, tokens.Count);
        for (var i = 0; i < tokens.Count; i++)
        {
            Assert.Equal(expected[i], tokens[i].TokenType);
        }
    }

    [Theory]
    [InlineData("\"hello\nworld\"")]
    [InlineData("\"string with\ttab\"")]
    [InlineData("\"string with \\n escape\"")]
    public void TheLexer_ShouldHandleStringsWithSpecialCharacters(string input)
    {
        var errorHandler = new ErrorHandler(input);
        var lexer = new Lexer(input, errorHandler);
        var tokens = lexer.Tokenize();

        Assert.Equal(2, tokens.Count);
        Assert.Equal(TokenType.StringLiteral, tokens.First().TokenType);
        Assert.Equal(input, tokens.First().Span.Slice(input));
    }

    [Fact]
    public void TheLexer_ShouldHandleVeryLongInput()
    {
        var longIdentifier = new string('a', 1000);
        var errorHandler = new ErrorHandler(longIdentifier);
        var lexer = new Lexer(longIdentifier, errorHandler);
        var tokens = lexer.Tokenize();

        Assert.Equal(2, tokens.Count);
        Assert.Equal(TokenType.Identifier, tokens.First().TokenType);
        Assert.Equal(longIdentifier, tokens.First().Span.Slice(longIdentifier));
    }

    [Theory]
    [InlineData("'a'", "a")]
    [InlineData("'0'", "0")]
    [InlineData("'*'", "*")]
    [InlineData("' '", " ")]
    [InlineData("'\n'", "\n")]
    [InlineData("'\\0'", "\0")]
    [InlineData("'\\n'", "\n")]
    [InlineData("'\\r'", "\r")]
    [InlineData("'\\t'", "\t")]
    [InlineData("'\\\\'", "\\")]
    [InlineData("'\\''", "'")]
    [InlineData("'\\\"'", "\"")]
    public void TheLexer_ShouldLexCharacterLiterals_WhenValid(string input, string expectedValue)
    {
        var errorHandler = new ErrorHandler(input);
        var lexer = new Lexer(input, errorHandler);
        var tokens = lexer.Tokenize();

        Assert.Equal(2, tokens.Count);
        Assert.Equal(TokenType.CharacterLiteral, tokens.First().TokenType);
        Assert.Equal(TokenType.Eof, tokens.Last().TokenType);
    }

    [Theory]
    [InlineData("'ab'")]
    [InlineData("''")]
    [InlineData("'\n\r'")]
    [InlineData("'\\q'")]
    [InlineData("  'a '")]
    [InlineData("'a")]
    public void TheLexer_ShouldProduceInvalidTokens_ForInvalidCharacterLiterals(string input)
    {
        var errorHandler = new ErrorHandler(input);
        var lexer = new Lexer(input, errorHandler);
        var tokens = lexer.Tokenize();

        Assert.Equal(2, tokens.Count);
        Assert.Equal(TokenType.Invalid, tokens.First().TokenType);
        Assert.Equal(TokenType.Eof, tokens.Last().TokenType);
    }
}
