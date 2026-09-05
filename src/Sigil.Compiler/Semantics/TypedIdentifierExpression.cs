namespace Sigil.Compiler.Semantics;

using Sigil.Compiler.Syntax.Expressions;

public sealed class TypedIdentifierExpression(
    IdentifierExpression expression,
    Symbol symbol,
    Type type)
    : TypedExpression(type)
{
    public IdentifierExpression Expression { get; } = expression;

    public Symbol Symbol { get; } = symbol;
}
