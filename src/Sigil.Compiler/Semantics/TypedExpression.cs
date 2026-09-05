namespace Sigil.Compiler.Semantics;

public abstract class TypedExpression(Type type)
{
    public Type Type { get; } = type;
}