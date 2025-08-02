using Sigil.Common;
using Sigil.Parsing;

namespace Sigil.Interpretation.Builtins;

public class PrintlnBuiltin : ICallable
{
    public string Name => "println";

    public int Arity => -1;

    public object? Call(Interpreter interpreter, List<object?> arguments, Span span)
    {
        var outputText = string.Join("", arguments);
        interpreter.OutputWriter.WriteLine(outputText);
        return null;
    }
}
