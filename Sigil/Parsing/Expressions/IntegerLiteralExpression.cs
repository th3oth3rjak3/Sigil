using Sigil.Common;

namespace Sigil.Parsing.Expressions;
public record IntegerLiteralExpression(long Literal, Span Span) : Expression(Span)
{
    public override T Accept<T>(IExpressionVisitor<T> visitor) =>
        visitor.VisitIntegerLiteralExpression(this);
}
