using Sigil.Common;
using Sigil.ErrorHandling;

namespace Sigil.Tests.ErrorHandling;

public class ErrorHandlerTests
{
    private const string SampleCode = "let x = 42;\nlet y = 'hello';\nif x > 10 {\n    print(y);\n}";

    [Fact]
    public void Report_AddsErrorWithCorrectFormat()
    {
        // Arrange
        var handler = new ErrorHandler(SampleCode);
        var span = new Span(new Position(2, 5, 20, 12), new Position(2, 11, 26, 12));

        // Act
        handler.Report("Unexpected token", span);

        // Assert
        Assert.Equal(3, handler.Errors.Count);
        Assert.StartsWith("[2:5] Error: Unexpected token", handler.Errors[0]);
    }

    [Fact]
    public void Report_GeneratesCorrectCodeContext_SingleLine()
    {
        // Arrange
        var handler = new ErrorHandler(SampleCode);
        var span = new Span(new Position(2, 10, 20, 12), new Position(2, 16, 26, 12));

        // Act
        handler.Report("Test error", span);
        var errors = handler.Errors;

        // Assert
        Assert.Equal(3, errors.Count);
        Assert.Equal("2 | let y = 'hello';", errors[1]);
        Assert.Equal("            ^^^^^^^ <- Error Here", errors[2]);
    }

    [Fact]
    public void HadError_Flag_SetCorrectly()
    {
        var handler = new ErrorHandler(SampleCode);
        Assert.False(handler.HadError);

        var span = new Span(new Position(1, 1, 0, 0), new Position(1, 1, 0, 0));
        handler.Report("Test", span);

        Assert.True(handler.HadError);
    }

    [Fact]
    public void Report_TokenSpan_ShowsPreciseErrorLocation()
    {
        // Arrange
        const string code = "let name = 'John';\nlet age = 30;";
        var handler = new ErrorHandler(code);

        // Calculate positions for "30" (line 2)
        int line2Start = 19;
        int tokenStart = 29;
        int tokenEnd = tokenStart + 1;

        var span = new Span(
            new Position(2, 11, tokenStart, line2Start),
            new Position(2, 12, tokenEnd, line2Start));

        // Act
        handler.Report("Numbers not allowed", span);

        // Assert
        var expected = new[]
        {
        "[2:11] Error: Numbers not allowed",
        "2 | let age = 30;",
        "              ^^ <- Error Here"  // Underlines "30"
    };

        Assert.Equal(expected, handler.Errors);
    }

    [Fact]
    public void Report_InvalidKeyword_ShowsExactLocation()
    {
        const string code = "fn main() {\n    retrn 5;\n}";
        var handler = new ErrorHandler(code);

        // Calculate positions for "retrn" (line 2)
        int line2Start = code.IndexOf("\n") + 1;
        int tokenStart = code.IndexOf("retrn");
        int tokenEnd = tokenStart + 5;

        var span = new Span(
            new Position(2, 5, tokenStart, line2Start),
            new Position(2, 9, tokenEnd, line2Start));

        handler.Report("Misspelled keyword", span);

        var expected = new[]
        {
        "[2:5] Error: Misspelled keyword",
        "2 |     retrn 5;",
        "        ^^^^^ <- Error Here"  // Marks "retrn"
    };

        Assert.Equal(expected, handler.Errors);
    }

    [Fact]
    public void Report_MissingSemicolon_ShowsEndOfLine()
    {
        const string code = "let x = 5\nlet y = 10;";
        var handler = new ErrorHandler(code);

        // Calculate positions for end of first line
        int line1Start = 0;
        int errorPos = 9;

        var span = new Span(
            new Position(1, 10, errorPos, line1Start),
            new Position(1, 10, errorPos, line1Start));

        handler.Report("Missing semicolon", span);

        var expected = new[]
        {
        "[1:10] Error: Missing semicolon",
        "1 | let x = 5",
        "             ^ <- Error Here"  // Points at end
    };

        Assert.Equal(expected, handler.Errors);
    }

    [Fact]
    public void Report_EmptySourceCode_HandlesGracefully()
    {
        // Arrange
        var handler = new ErrorHandler("");
        var span = new Span(new Position(1, 1, 0, 0), new Position(1, 1, 0, 0));

        // Act
        handler.Report("Empty source error", span);

        // Assert
        Assert.Equal("1 | ", handler.Errors[1]);
        Assert.Equal("    ^ <- Error Here", handler.Errors[2]);
    }

    [Fact]
    public void Report_LineStartPosition_CorrectAlignment()
    {
        // Arrange
        const string code = "function() {\n  return 42;\n}";
        var handler = new ErrorHandler(code);
        int line2Start = 13;

        var span = new Span(
            new Position(2, 1, line2Start, line2Start),
            new Position(2, 2, line2Start + 1, line2Start));

        // Act
        handler.Report("Indentation error", span);

        // Assert
        Assert.Equal("2 |   return 42;", handler.Errors[1]);
        Assert.Equal("    ^^ <- Error Here", handler.Errors[2]);
    }

    [Fact]
    public void Report_LineEndPosition_CorrectAlignment()
    {
        // Arrange
        const string code = "let x = 5;\nlet y = 10;";
        var handler = new ErrorHandler(code);
        int line1Start = 0;
        int semicolonPos = code.IndexOf(';');

        var span = new Span(
            new Position(1, 10, semicolonPos, line1Start),
            new Position(1, 10, semicolonPos, line1Start));

        // Act
        handler.Report("Semicolon issue", span);

        // Assert
        Assert.Equal("1 | let x = 5;", handler.Errors[1]);
        Assert.Equal("             ^ <- Error Here", handler.Errors[2]);
    }

    [Fact]
    public void Report_TabCharacters_CorrectAlignment()
    {
        // Arrange
        const string code = "if true {\n\treturn 42;\n}";
        var handler = new ErrorHandler(code);
        int line2Start = 10;
        int tabPos = 10;

        var span = new Span(
            new Position(2, 1, tabPos, line2Start),
            new Position(2, 1, tabPos, line2Start));

        // Act
        handler.Report("Tab indentation", span);

        // Assert
        Assert.Equal("2 | \treturn 42;", handler.Errors[1]);
        Assert.Equal("    ^ <- Error Here", handler.Errors[2]);
    }

    [Fact]
    public void Report_MultiByteCharacters_HandlesAsSingleChars()
    {
        // Arrange
        const string code = "let π = 3.14;";
        var handler = new ErrorHandler(code);

        var span = new Span(
            new Position(1, 5, code.IndexOf('π'), 0),
            new Position(1, 5, code.IndexOf('π'), 0));

        // Act
        handler.Report("Special character", span);

        // Assert
        Assert.Equal("1 | let π = 3.14;", handler.Errors[1]);
        Assert.Equal("        ^ <- Error Here", handler.Errors[2]);
    }

    [Fact]
    public void Report_BackwardsSpan_UsesStartPosition()
    {
        // Arrange
        const string code = "let x = 5;";
        var handler = new ErrorHandler(code);

        Assert.Throws<ArgumentException>(
            () => new Span(
            new Position(1, 8, 7, 0),  // Start at '5'
            new Position(1, 5, 4, 0))); // End at 'x' (backwards)
    }

    [Fact]
    public void Report_InvalidPositions_ThrowsException()
    {
        // Arrange
        var handler = new ErrorHandler("let x = 5;");

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            handler.Report("Test", new Span(
                new Position(0, 1, 0, 0),  // Invalid line
                new Position(1, 1, 0, 0))));

        Assert.Throws<ArgumentException>(() =>
            handler.Report("Test", new Span(
                new Position(1, 0, 0, 0),  // Invalid column
                new Position(1, 1, 0, 0))));
    }

    [Fact]
    public void Report_MultipleErrors_MaintainsOrder()
    {
        // Arrange
        const string code = "let x = 5\nlet y = 10;";
        var handler = new ErrorHandler(code);

        // First error (missing semicolon)
        var span1 = new Span(
            new Position(1, 9, code.IndexOf('5'), 0),
            new Position(1, 10, code.IndexOf('5') + 1, 0));

        // Second error (valid)
        var span2 = new Span(
            new Position(2, 5, code.IndexOf('y'), code.IndexOf('\n') + 1),
            new Position(2, 6, code.IndexOf('y') + 1, code.IndexOf('\n') + 1));

        // Act
        handler.Report("First error", span1);
        handler.Report("Second error", span2);

        // Assert
        Assert.Equal(6, handler.Errors.Count);
        Assert.Contains("First error", handler.Errors[0]);
        Assert.Contains("Second error", handler.Errors[3]);
    }

    [Fact]
    public void Report_EndOfFilePosition_HandlesGracefully()
    {
        // Arrange
        const string code = "let x = 5";
        var handler = new ErrorHandler(code);
        int eofPos = code.Length;

        var span = new Span(
            new Position(1, 10, eofPos, 0),
            new Position(1, 10, eofPos, 0));

        // Act
        handler.Report("EOF error", span);

        // Assert
        Assert.Equal("1 | let x = 5", handler.Errors[1]);
        Assert.Equal("             ^ <- Error Here", handler.Errors[2]);
    }
}
