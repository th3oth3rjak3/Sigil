﻿using Sigil.Common;
using Sigil.Parsing.Expressions;

namespace Sigil.Parsing.Statements;

public record IfStatement(Expression Condition, Statement ThenBranch, Statement? ElseBranch, Span Span) : Statement(Span)
{
    public override T Accept<T>(IStatementVisitor<T> visitor) =>
        visitor.VisitIfStatement(this);
}
