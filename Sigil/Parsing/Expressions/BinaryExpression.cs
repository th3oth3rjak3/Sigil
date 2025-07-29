using Sigil.Common;
using Sigil.Lexing;

namespace Sigil.Parsing.Expressions;

public record BinaryExpression(Expression Left, Token Operator, Expression Right, Span Span) : Expression(Span)
{
    public override T Accept<T>(IExpressionVisitor<T> visitor) =>
        visitor.VisitBinaryExpression(this);
}