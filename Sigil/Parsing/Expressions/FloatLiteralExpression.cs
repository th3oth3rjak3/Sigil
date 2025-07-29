using Sigil.Common;

namespace Sigil.Parsing.Expressions;
public record FloatLiteralExpression(double Literal, Span Span) : Expression(Span)
{
    public override T Accept<T>(IExpressionVisitor<T> visitor) =>
        visitor.VisitFloatLiteralExpression(this);
}
