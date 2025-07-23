namespace Sigil.Common;

/// <summary>
/// Position represents the Line, Column, and Offset in some source code.
/// </summary>
public record Position
{
    /// <summary>
    /// The line number that the source code was found on, starting with 1.
    /// </summary>
    public int Line { get; }

    /// <summary>
    /// The column in the current line that the source code started, starting with column 1.
    /// </summary>
    public int Column { get; }

    /// <summary>
    /// The distance from the beginning of the source code in bytes, starting with 0.
    /// </summary>
    public int Offset { get; }

    /// <summary>
    /// The offset for the start of the line used in error reporting.
    /// </summary>
    public int LineOffset { get; }

    public Position(int Line, int Column, int Offset, int LineOffset)
    {
        if (Line < 1) throw new ArgumentException("Line must be positive", nameof(Line));
        if (Column < 1) throw new ArgumentException("Column must be positive", nameof(Column));
        if (Offset < 0) throw new ArgumentException("Offset cannot be negative", nameof(Offset));
        if (LineOffset < 0 || LineOffset > Offset) throw new ArgumentException("Line Offset cannot be negative or greater than the current offset");

        this.Line = Line;
        this.Column = Column;
        this.Offset = Offset;
        this.LineOffset = LineOffset;
    }
}
