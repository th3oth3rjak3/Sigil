using Sigil.Common;

namespace Sigil.Parsing.Statements;

public record FunctionStatement(string Name, List<string> Parameters, List<Statement> Body, Span Span)
    : Statement(Span)
{
    public override T Accept<T>(IStatementVisitor<T> visitor) =>
        visitor.VisitFunctionStatement(this);
}
