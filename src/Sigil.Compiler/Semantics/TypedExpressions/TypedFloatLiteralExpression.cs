using Sigil.Compiler.Semantics.Types;
using Sigil.Compiler.Syntax.Expressions;

namespace Sigil.Compiler.Semantics.TypedExpressions;

public sealed class TypedFloatLiteralExpression(
    FloatLiteralExpression expression)
    : TypedExpression(new FloatType())
{
    public FloatLiteralExpression Expression { get; } = expression;
}
