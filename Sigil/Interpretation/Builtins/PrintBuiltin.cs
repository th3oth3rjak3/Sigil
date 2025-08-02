using Sigil.Common;
using Sigil.Parsing;

namespace Sigil.Interpretation.Builtins;

public class PrintBuiltin : ICallable
{
    public string Name => "print";

    public int Arity => 1;

    public object? Call(Interpreter interpreter, List<object?> arguments, Span span)
    {
        interpreter.OutputWriter.WriteLine(arguments[0]);
        return null;
    }
}
