using Sigil.Common;

namespace Sigil.Parsing.Expressions;
public record IdentifierExpression(string Name, Span Span) : Expression(Span)
{
    public override T Accept<T>(IExpressionVisitor<T> visitor) =>
        visitor.VisitIdentifierExpression(this);
}
