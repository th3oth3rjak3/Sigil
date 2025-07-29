using Sigil.Common;

namespace Sigil.Parsing.Expressions;
public record BooleanLiteralExpression(bool Literal, Span Span) : Expression(Span)
{
    public override T Accept<T>(IExpressionVisitor<T> visitor) =>
        visitor.VisitBooleanLiteralExpression(this);
}
