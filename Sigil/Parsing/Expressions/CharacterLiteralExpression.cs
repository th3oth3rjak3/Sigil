﻿using Sigil.Common;

namespace Sigil.Parsing.Expressions;

public record CharacterLiteralExpression(char Value, Span Span) : Expression(Span)
{
    public override T Accept<T>(IExpressionVisitor<T> visitor) =>
        visitor.VisitCharacterLiteralExpression(this);
}
