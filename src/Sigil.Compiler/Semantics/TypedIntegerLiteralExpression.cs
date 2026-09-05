namespace Sigil.Compiler.Semantics;

using Sigil.Compiler.Syntax;

public sealed class TypedIntegerLiteralExpression(
    IntegerLiteralExpression expression)
    : TypedExpression(new IntegerType())
{
    public IntegerLiteralExpression Expression { get; } = expression;
}