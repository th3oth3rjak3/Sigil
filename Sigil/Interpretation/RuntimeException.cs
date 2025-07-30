using Sigil.Common;

namespace Sigil.Interpretation;

public class RuntimeException : Exception
{
    public Span Span { get; }

    public RuntimeException(string message, Span span) : base(message)
    {
        Span = span;
    }
}