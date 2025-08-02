using Sigil.Common;
using Sigil.Parsing;

namespace Sigil.Interpretation.Builtins;

public class PrintBuiltin : ICallable
{
    public string Name => "print";

    public int Arity => -1;

    public object? Call(Interpreter interpreter, List<object?> arguments, Span span)
    {
        var outputText = string.Join("", arguments);
        interpreter.OutputWriter.Write(outputText);
        return null;
    }
}
