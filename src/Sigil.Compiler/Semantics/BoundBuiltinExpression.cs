namespace Sigil.Compiler.Semantics;

public sealed class BoundBuiltinExpression(
    IReadOnlyList<Builtin> candidates)
    : BoundExpression
{
    public IReadOnlyList<Builtin> Candidates { get; } = candidates;
}
