using Sigil.Common;

namespace Sigil.Parsing.Statements;
public record BlockStatement(List<Statement> Statements, Span Span) : Statement(Span)
{
    public override T Accept<T>(IStatementVisitor<T> visitor) =>
        visitor.VisitBlockStatement(this);
}
