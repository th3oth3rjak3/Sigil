namespace Sigil.Compiler.Semantics;

public sealed class BuiltinRegistry
{
    private readonly Dictionary<string, IReadOnlyList<Builtin>> _builtins = new()
    {
        ["println"] =
        [
            new Builtin(
                "println",
                [new IntegerType()],
                new VoidType(),
                "sigil_println_integer"),

            new Builtin(
                "println",
                [new FloatType()],
                new VoidType(),
                "sigil_println_float"),
        ],
    };

    public bool TryGet(
        string name,
        out IReadOnlyList<Builtin> builtins)
    {
        if (_builtins.TryGetValue(name, out var found))
        {
            builtins = found;
            return true;
        }

        builtins = [];
        return false;
    }
}