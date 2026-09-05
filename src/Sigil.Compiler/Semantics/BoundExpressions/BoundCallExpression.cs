using Sigil.Compiler.Syntax.Expressions;

namespace Sigil.Compiler.Semantics.BoundExpressions;

public sealed class BoundCallExpression(
    CallExpression expression,
    BoundIdentifierExpression callee,
    IReadOnlyList<BoundExpression> arguments)
    : BoundExpression
{
    public CallExpression Expression { get; } = expression;
    public BoundIdentifierExpression Callee { get; } = callee;
    public IReadOnlyList<BoundExpression> Arguments { get; } = arguments;
}
