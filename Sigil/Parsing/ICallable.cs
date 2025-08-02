using Sigil.Common;
using Sigil.Interpretation;

namespace Sigil.Parsing;

public interface ICallable
{
    public string Name { get; }
    int Arity { get; }
    object? Call(Interpreter interpreter, List<object?> arguments, Span span);
}
