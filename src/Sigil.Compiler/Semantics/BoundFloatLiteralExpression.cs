namespace Sigil.Compiler.Semantics;

using Sigil.Compiler.Syntax.Expressions;

public sealed class BoundFloatLiteralExpression(
    FloatLiteralExpression expression)
    : BoundExpression
{
    public FloatLiteralExpression Expression { get; } = expression;
}
