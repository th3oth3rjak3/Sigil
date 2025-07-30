using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Sigil.Common;
using Sigil.Parsing.Expressions;

namespace Sigil.Parsing.Statements;
public record IfStatement(Expression Condition, Statement ThenBranch, Statement? ElseBranch, Span Span) : Statement(Span)
{
    public override T Accept<T>(IStatementVisitor<T> visitor) =>
        visitor.VisitIfStatement(this);
}
