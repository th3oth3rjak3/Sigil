namespace Sigil.Compiler.Semantics;

using Sigil.Compiler.Syntax;

public sealed class BoundFloatLiteralExpression(
    FloatLiteralExpression expression)
    : BoundExpression
{
    public FloatLiteralExpression Expression { get; } = expression;
}
