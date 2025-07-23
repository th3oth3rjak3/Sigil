using Sigil.Lexing;

namespace Sigil.Tests.Lexing;

public class KeywordsTests
{
    [Theory]
    [InlineData("let", TokenType.Let)]
    [InlineData("fun", TokenType.Fun)]
    [InlineData("class", TokenType.Class)]
    [InlineData("new", TokenType.New)]
    [InlineData("this", TokenType.This)]
    [InlineData("if", TokenType.If)]
    [InlineData("else", TokenType.Else)]
    [InlineData("while", TokenType.While)]
    [InlineData("for", TokenType.For)]
    [InlineData("return", TokenType.Return)]
    [InlineData("true", TokenType.True)]
    [InlineData("false", TokenType.False)]
    [InlineData("break", TokenType.Break)]
    [InlineData("continue", TokenType.Continue)]
    [InlineData("or", TokenType.Or)]
    [InlineData("and", TokenType.And)]
    public void FromString_ReturnsSomeTokenType_ForKeywords(string keyword, TokenType expectedType)
    {
        // Act
        var result = Keywords.FromString(keyword);

        // Assert
        Assert.True(result.IsSome);
        Assert.Equal(expectedType, result.Unwrap());
    }

    [Theory]
    [InlineData("")]
    [InlineData("LET")] // case sensitive
    [InlineData("iff")] // partial match
    [InlineData("function")]
    [InlineData("123")]
    [InlineData("+")]
    public void FromString_ReturnsNone_ForNonKeywords(string input)
    {
        // Act
        var result = Keywords.FromString(input);

        // Assert
        Assert.True(result.IsNone);
    }

    [Fact]
    public void FromString_IsCaseSensitive()
    {
        // Act & Assert
        Assert.True(Keywords.FromString("let").IsSome);
        Assert.True(Keywords.FromString("Let").IsNone);
        Assert.True(Keywords.FromString("LET").IsNone);
    }

    [Fact]
    public void FromString_HandlesNullInput()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => Keywords.FromString(null!));
    }
}
