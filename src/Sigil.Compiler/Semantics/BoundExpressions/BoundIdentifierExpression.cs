using Sigil.Compiler.Semantics.Primitives;
using Sigil.Compiler.Syntax.Expressions;

namespace Sigil.Compiler.Semantics.BoundExpressions;

public sealed class BoundIdentifierExpression(IdentifierExpression expression, Symbol symbol): BoundExpression
{
    public IdentifierExpression Expression { get; } = expression;
    public Symbol Symbol { get; } = symbol;
}
