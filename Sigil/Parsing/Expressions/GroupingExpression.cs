using Sigil.Common;

namespace Sigil.Parsing.Expressions;
public record GroupingExpression(Expression Expression, Span Span) : Expression(Span)
{
    public override T Accept<T>(IExpressionVisitor<T> visitor) =>
        visitor.VisitGroupingExpression(this);
}
