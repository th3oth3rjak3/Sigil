using Sigil.Compiler.Syntax;

namespace Sigil.Compiler.Semantics;

public sealed class BoundBinaryExpression(
    BinaryExpression expression,
    BoundExpression left,
    BoundExpression right)
    : BoundExpression
{
    public BinaryExpression Expression { get; } = expression;
    public BoundExpression Left { get; } = left;
    public BoundExpression Right { get; } = right;
}
