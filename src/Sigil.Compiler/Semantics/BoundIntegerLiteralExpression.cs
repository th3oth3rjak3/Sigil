namespace Sigil.Compiler.Semantics;

using Sigil.Compiler.Syntax.Expressions;

public sealed class BoundIntegerLiteralExpression(IntegerLiteralExpression expression): BoundExpression
{
    public IntegerLiteralExpression Expression { get; } = expression;
}
