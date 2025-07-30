using Sigil.Common;

namespace Sigil.Parsing.Expressions;

public record StringLiteralExpression(string Value, Span Span) : Expression(Span)
{
    public override T Accept<T>(IExpressionVisitor<T> visitor) =>
        visitor.VisitStringLiteralExpression(this);
}
