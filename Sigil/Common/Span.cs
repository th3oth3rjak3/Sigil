namespace Sigil.Common;

/// <summary>
/// Span represents the start and end positions of some source code.
/// </summary>
/// <param name="Start">The position in the source code where the span starts.</param>
/// <param name="End">The position in the source code where the span ends.</param>
public record Span
{
    public Span(Position Start, Position End)
    {
        if (Start.Offset > End.Offset) throw new ArgumentException("Start position must be before the End position.");

        this.Start = Start;
        this.End = End;
    }

    /// <summary>
    /// The start of the span.
    /// </summary>
    public Position Start { get; set; }

    /// <summary>
    /// The end of the span.
    /// </summary>
    public Position End { get; set; }

    /// <summary>
    /// Checks if a character at the given offset is within the span (inclusive)
    /// </summary>
    public bool ContainsOffset(int offset) =>
        Start.Offset <= offset && End.Offset >= offset;

    /// <summary>
    /// Returns a substring of the source code covered by this span
    /// </summary>
    public string Slice(string sourceCode) =>
        sourceCode[Start.Offset..(End.Offset + 1)];

    /// <summary>
    /// Merges two spans to produce one that covers both (even if disjoint)
    /// </summary>
    public Span Merge(Span other) =>
        new(
            Start.Offset < other.Start.Offset ? Start : other.Start,
            End.Offset > other.End.Offset ? End : other.End
        );
}
