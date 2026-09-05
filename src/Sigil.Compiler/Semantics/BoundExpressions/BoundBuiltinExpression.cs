using Sigil.Compiler.Semantics.Primitives;

namespace Sigil.Compiler.Semantics.BoundExpressions;

public sealed class BoundBuiltinExpression(
    IReadOnlyList<Builtin> candidates)
    : BoundExpression
{
    public IReadOnlyList<Builtin> Candidates { get; } = candidates;
}
