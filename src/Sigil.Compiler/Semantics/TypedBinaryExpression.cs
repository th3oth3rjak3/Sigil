using Sigil.Compiler.Syntax.Expressions;

namespace Sigil.Compiler.Semantics;

public sealed class TypedBinaryExpression(
    BinaryExpression expression,
    TypedExpression left,
    TypedExpression right,
    Type type)
    : TypedExpression(type)
{
    public BinaryExpression Expression { get; } = expression;
    public TypedExpression Left { get; } = left;
    public TypedExpression Right { get; } = right;
}
