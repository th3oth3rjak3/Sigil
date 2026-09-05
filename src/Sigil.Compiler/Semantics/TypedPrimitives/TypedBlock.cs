using Sigil.Compiler.Semantics.TypedStatements;

namespace Sigil.Compiler.Semantics.TypedPrimitives;

public sealed class TypedBlock(
    IReadOnlyList<TypedStatement> statements)
{
    public IReadOnlyList<TypedStatement> Statements { get; }
        = statements;
}
