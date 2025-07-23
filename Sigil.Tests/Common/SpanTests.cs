using Sigil.Common;

namespace Sigil.Tests.Common;

public class SpanTests
{
    [Fact]
    public void Span_HasPositionAccessors()
    {
        var start = new Position(1, 1, 0, 0);
        var end = new Position(1, 5, 4, 0);
        var span = new Span(start, end);

        Assert.Equal(start, span.Start);
        Assert.Equal(end, span.End);
    }

    [Fact]
    public void Span_ContainsOffset()
    {
        var start = new Position(1, 1, 0, 0);
        var end = new Position(1, 5, 4, 0);
        var span = new Span(start, end);

        for (int i = 0; i <= 4; i++)
        {
            Assert.True(span.ContainsOffset(i));
        }

        Assert.False(span.ContainsOffset(5));
    }

    [Fact]
    public void Span_SlicesSourceCode()
    {
        var sourceCode = "let myInt: Int = 0;";
        var start = new Position(1, 1, 0, 0);
        var end = new Position(1, 5, 4, 0);
        var span = new Span(start, end);
        var slice = span.Slice(sourceCode);
        Assert.Equal("let m", slice);
    }

    [Fact]
    public void Span_CanMergeWithOverlapping()
    {
        var start1 = new Position(1, 1, 0, 0);
        var end1 = new Position(1, 5, 4, 0);
        var span1 = new Span(start1, end1);

        var start2 = new Position(1, 3, 2, 0);
        var end2 = new Position(1, 8, 7, 0);
        var span2 = new Span(start2, end2);

        var merged = span1.Merge(span2);

        Assert.Equal(0, merged.Start.Offset);
        Assert.Equal(7, merged.End.Offset);
    }

    [Fact]
    public void Span_CanMergeWithDisjointSpan()
    {
        var start1 = new Position(1, 1, 0, 0);
        var end1 = new Position(1, 5, 4, 0);
        var span1 = new Span(start1, end1);

        var start2 = new Position(1, 7, 6, 0);
        var end2 = new Position(1, 10, 9, 0);
        var span2 = new Span(start2, end2);

        var merged = span1.Merge(span2);

        Assert.Equal(0, merged.Start.Offset);
        Assert.Equal(9, merged.End.Offset);
    }

    [Theory]
    [InlineData(5, 10, 15)] // Within span
    [InlineData(5, 5, 5)]   // Exactly at start
    [InlineData(15, 15, 15)] // Exactly at end
    public void ContainsOffset_ReturnsTrue_WhenOffsetWithinSpan(int startOffset, int testOffset, int endOffset)
    {
        // Arrange
        var span = new Span(
            new Position(1, 1, startOffset, 0),
            new Position(1, 5, endOffset, 0));

        // Act & Assert
        Assert.True(span.ContainsOffset(testOffset));
    }

    [Theory]
    [InlineData(5, 4)] // Before start
    [InlineData(15, 16)] // After end
    public void ContainsOffset_ReturnsFalse_WhenOffsetOutsideSpan(int spanEndOffset, int testOffset)
    {
        // Arrange
        var span = new Span(
            new Position(1, 1, 5, 0),
            new Position(1, 5, spanEndOffset, 0));

        // Act & Assert
        Assert.False(span.ContainsOffset(testOffset));
    }

    [Fact]
    public void Merge_SelectsMinStartAndMaxEnd_ForDisjointSpans()
    {
        // Arrange
        var span1 = new Span(new Position(1, 1, 5, 0), new Position(1, 5, 10, 0));
        var span2 = new Span(new Position(1, 1, 3, 0), new Position(1, 8, 15, 0));

        // Act
        var merged = span1.Merge(span2);

        // Assert
        Assert.Equal(3, merged.Start.Offset); // Should take span2's start
        Assert.Equal(15, merged.End.Offset); // Should take span2's end
    }

    [Fact]
    public void Merge_SelectsCorrectPositions_WhenOneSpanContainsAnother()
    {
        // Arrange
        var outer = new Span(new Position(1, 1, 0, 0), new Position(3, 5, 50, 45));
        var inner = new Span(new Position(2, 1, 20, 10), new Position(2, 5, 30, 10));

        // Act
        var merged = outer.Merge(inner);

        // Assert
        Assert.Equal(0, merged.Start.Offset); // Should keep outer's start
        Assert.Equal(50, merged.End.Offset); // Should keep outer's end
    }

    [Fact]
    public void Merge_HandlesIdenticalSpans()
    {
        // Arrange
        var span = new Span(new Position(1, 1, 0, 0), new Position(1, 5, 4, 0));

        // Act
        var merged = span.Merge(span);

        // Assert
        Assert.Equal(span, merged);
    }
}

public class SpannedTests
{
    [Fact]
    public void Spanned_StoresNodeAndSpan()
    {
        var node = "test";
        var span = new Span(new Position(1, 1, 0, 0), new Position(1, 4, 3, 0));
        var spanned = new Spanned<string>(node, span);

        Assert.Equal(node, spanned.Node);
        Assert.Equal(span, spanned.Span);
    }
}
