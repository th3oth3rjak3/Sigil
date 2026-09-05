using Sigil.Compiler.Semantics.BoundStatements;

namespace Sigil.Compiler.Semantics.BoundPrimitives;

public sealed class BoundBlock(IReadOnlyList<BoundStatement> statements)
{
    public IReadOnlyList<BoundStatement> Statements { get; } = statements;
}
