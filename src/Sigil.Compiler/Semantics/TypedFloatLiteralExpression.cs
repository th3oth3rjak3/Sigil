namespace Sigil.Compiler.Semantics;

using Sigil.Compiler.Syntax;

public sealed class TypedFloatLiteralExpression(
    FloatLiteralExpression expression)
    : TypedExpression(new FloatType())
{
    public FloatLiteralExpression Expression { get; } = expression;
}
