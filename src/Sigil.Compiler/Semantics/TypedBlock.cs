namespace Sigil.Compiler.Semantics;

public sealed class TypedBlock(
    IReadOnlyList<TypedStatement> statements)
{
    public IReadOnlyList<TypedStatement> Statements { get; }
        = statements;
}
