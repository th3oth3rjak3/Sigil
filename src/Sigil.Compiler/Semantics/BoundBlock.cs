namespace Sigil.Compiler.Semantics;

public sealed class BoundBlock(IReadOnlyList<BoundStatement> statements)
{
    public IReadOnlyList<BoundStatement> Statements { get; } = statements;
}
