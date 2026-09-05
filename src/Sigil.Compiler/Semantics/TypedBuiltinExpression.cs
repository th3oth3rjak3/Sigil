namespace Sigil.Compiler.Semantics;

public sealed class TypedBuiltinExpression(
    Builtin builtin,
    Type type)
    : TypedExpression(type)
{
    public Builtin Builtin { get; } = builtin;
}
