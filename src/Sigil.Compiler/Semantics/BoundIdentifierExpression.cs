using Sigil.Compiler.Syntax.Expressions;

namespace Sigil.Compiler.Semantics;

public sealed class BoundIdentifierExpression(IdentifierExpression expression, Symbol symbol): BoundExpression
{
    public IdentifierExpression Expression { get; } = expression;
    public Symbol Symbol { get; } = symbol;
}
