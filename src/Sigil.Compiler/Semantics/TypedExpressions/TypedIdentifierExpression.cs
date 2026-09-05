using Sigil.Compiler.Semantics.Primitives;
using Sigil.Compiler.Semantics.Types;
using Sigil.Compiler.Syntax.Expressions;

namespace Sigil.Compiler.Semantics.TypedExpressions;

public sealed class TypedIdentifierExpression(
    IdentifierExpression expression,
    Symbol symbol,
    SigilType type)
    : TypedExpression(type)
{
    public IdentifierExpression Expression { get; } = expression;

    public Symbol Symbol { get; } = symbol;
}
