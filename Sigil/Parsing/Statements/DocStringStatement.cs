using System;
using Sigil.Common;

namespace Sigil.Parsing.Statements;

public record DocStringStatement(string Text, Span Span) : Statement(Span)
{
    public override T Accept<T>(IStatementVisitor<T> visitor)
    {
        return visitor.VisitDocStringStatement(this);
    }
}
