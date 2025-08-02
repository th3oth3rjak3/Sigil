using Sigil.Common;
using Sigil.Parsing;

namespace Sigil.Interpretation.Builtins;

public class StringBuiltin : ICallable
{
    public string Name => "string";

    public int Arity => 1;

    public object? Call(Interpreter interpreter, List<object?> arguments, Span span)
    {
        if (arguments.Count == 0)
        {
            return null;
        }

        return arguments[0]?.ToString();
    }
}
