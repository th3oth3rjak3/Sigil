namespace Sigil.Common;

/// <summary>
/// A generic container that associates a value with its source code location.
/// </summary>
public record Spanned<T>(T Node, Span Span);
