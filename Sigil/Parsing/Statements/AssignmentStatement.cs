using Sigil.Common;
using Sigil.Parsing.Expressions;

namespace Sigil.Parsing.Statements;
public record AssignmentStatement(string Name, Expression Value, Span Span) : Statement(Span)
{
    public override T Accept<T>(IStatementVisitor<T> visitor) =>
        visitor.VisitAssignmentStatement(this);
}
