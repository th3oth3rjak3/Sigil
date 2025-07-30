using Sigil.Common;

namespace Sigil.Parsing.Expressions;
public record CallExpression(Expression Callee, List<Expression> Arguments, Span Span) : Expression(Span)
{
    public override T Accept<T>(IExpressionVisitor<T> visitor) =>
        visitor.VisitCallExpression(this);
}
