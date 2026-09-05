using Sigil.Compiler.Syntax.Expressions;

namespace Sigil.Compiler.Semantics.BoundExpressions;

public sealed class BoundIntegerLiteralExpression(IntegerLiteralExpression expression): BoundExpression
{
    public IntegerLiteralExpression Expression { get; } = expression;
}
