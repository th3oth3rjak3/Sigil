using Sigil.Common;

namespace Sigil.Tests.Common;

public class PositionTests
{
    [Fact]
    public void Position_Constructor_SetsPropertiesCorrectly()
    {
        // Arrange & Act
        var position = new Position(10, 5, 100, 20);

        // Assert
        Assert.Equal(10, position.Line);
        Assert.Equal(5, position.Column);
        Assert.Equal(100, position.Offset);
        Assert.Equal(20, position.LineOffset);
    }

    [Fact]
    public void Position_Equality_WorksCorrectly()
    {
        // Arrange
        var pos1 = new Position(1, 1, 0, 0);
        var pos2 = new Position(1, 1, 0, 0);
        var pos3 = new Position(2, 1, 10, 8);

        // Assert
        Assert.Equal(pos1, pos2);
        Assert.NotEqual(pos1, pos3);
        Assert.True(pos1 == pos2);
        Assert.False(pos1 == pos3);
    }

    [Fact]
    public void Position_ToString_ReturnsRecordDefaultFormat()
    {
        // Arrange
        var position = new Position(5, 10, 42, 35);

        // Act
        var result = position.ToString();

        // Assert
        Assert.Equal("Position { Line = 5, Column = 10, Offset = 42, LineOffset = 35 }", result);
    }

    [Theory]
    [InlineData(0, 1, 0)] // line 0 should be invalid
    [InlineData(1, 0, 0)] // column 0 should be invalid
    [InlineData(1, 1, -1)] // negative offset
    public void Position_InvalidValues_ThrowException(int line, int column, int offset)
    {
        // Arrange, Act & Assert
        Assert.Throws<ArgumentException>(() => new Position(line, column, offset, 0));
    }
}
