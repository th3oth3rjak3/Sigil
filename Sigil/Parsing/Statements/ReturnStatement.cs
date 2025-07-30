using Sigil.Common;
using Sigil.Parsing.Expressions;

namespace Sigil.Parsing.Statements;
public record ReturnStatement(Expression? Expression, Span Span) : Statement(Span)
{
    public override T Accept<T>(IStatementVisitor<T> visitor) =>
        visitor.VisitReturnStatement(this);
}