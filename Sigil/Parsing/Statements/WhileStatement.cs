using Sigil.Common;
using Sigil.Parsing.Expressions;

namespace Sigil.Parsing.Statements;
public record WhileStatement(Expression Condition, Statement Body, Span Span) : Statement(Span)
{
    public override T Accept<T>(IStatementVisitor<T> visitor) =>
        visitor.VisitWhileStatement(this);
}
