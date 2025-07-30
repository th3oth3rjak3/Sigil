using Sigil.Common;

namespace Sigil.Parsing.Expressions;
public record BooleanLiteralExpression(bool Value, Span Span) : Expression(Span)
{
    public override T Accept<T>(IExpressionVisitor<T> visitor) =>
        visitor.VisitBooleanLiteralExpression(this);
}
