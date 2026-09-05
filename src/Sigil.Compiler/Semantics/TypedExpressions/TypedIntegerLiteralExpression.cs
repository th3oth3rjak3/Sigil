using Sigil.Compiler.Semantics.Types;
using Sigil.Compiler.Syntax.Expressions;

namespace Sigil.Compiler.Semantics.TypedExpressions;

public sealed class TypedIntegerLiteralExpression(
    IntegerLiteralExpression expression)
    : TypedExpression(new IntegerType())
{
    public IntegerLiteralExpression Expression { get; } = expression;
}
