using Sigil.Common;

namespace Sigil.Parsing.Expressions;

public abstract record Expression(Span Span) : AstNode(Span)
{
    public abstract T Accept<T>(IExpressionVisitor<T> visitor);
}
