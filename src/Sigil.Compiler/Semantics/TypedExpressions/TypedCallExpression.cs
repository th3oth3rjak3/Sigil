using Sigil.Compiler.Semantics.BoundExpressions;
using Sigil.Compiler.Semantics.Types;

namespace Sigil.Compiler.Semantics.TypedExpressions;

public sealed class TypedCallExpression(
    BoundCallExpression expression,
    SigilType type,
    IReadOnlyList<TypedExpression> arguments)
    : TypedExpression(type)
{
    public BoundCallExpression Expression { get; } = expression;
    public IReadOnlyList<TypedExpression> Arguments { get; } = arguments;
}
