using Sigil.Common;
using Sigil.Parsing.Expressions;

namespace Sigil.Parsing.Statements;

public record LetStatement(string Name, string? TypeName, Expression Initializer, Span Span) : Statement(Span)
{
    public override T Accept<T>(IStatementVisitor<T> visitor) =>
        visitor.VisitLetStatement(this);
}
