namespace Sigil.Compiler.Semantics;

using Sigil.Compiler.Syntax.Expressions;

public sealed class TypedFloatLiteralExpression(
    FloatLiteralExpression expression)
    : TypedExpression(new FloatType())
{
    public FloatLiteralExpression Expression { get; } = expression;
}
