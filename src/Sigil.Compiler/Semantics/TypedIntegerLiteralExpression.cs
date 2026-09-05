namespace Sigil.Compiler.Semantics;

using Sigil.Compiler.Syntax.Expressions;

public sealed class TypedIntegerLiteralExpression(
    IntegerLiteralExpression expression)
    : TypedExpression(new IntegerType())
{
    public IntegerLiteralExpression Expression { get; } = expression;
}
