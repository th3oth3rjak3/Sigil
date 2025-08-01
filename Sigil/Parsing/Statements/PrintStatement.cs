using Sigil.Common;
using Sigil.Parsing.Expressions;

namespace Sigil.Parsing.Statements;

public record PrintStatement(Expression Expression, Span Span) : Statement(Span)
{
    public override T Accept<T>(IStatementVisitor<T> visitor)
    {
        return visitor.VisitPrintStatement(this);
    }
}
