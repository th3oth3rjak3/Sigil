using Sigil.Common;

namespace Sigil.Parsing.Statements;

public record ClassStatement(string Name, List<FunctionStatement> Methods, List<FieldDeclaration> Fields, Span Span, string? SuperclassName = null) : Statement(Span)
{
    public override T Accept<T>(IStatementVisitor<T> visitor)
    {
        return visitor.VisitClassStatement(this);
    }
}
