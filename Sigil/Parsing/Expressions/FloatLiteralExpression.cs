using Sigil.Common;

namespace Sigil.Parsing.Expressions;
public record FloatLiteralExpression(double Value, Span Span) : Expression(Span)
{
    public override T Accept<T>(IExpressionVisitor<T> visitor) =>
        visitor.VisitFloatLiteralExpression(this);
}
