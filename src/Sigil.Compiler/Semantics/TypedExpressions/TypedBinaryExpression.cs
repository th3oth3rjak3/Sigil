using Sigil.Compiler.Semantics.Types;
using Sigil.Compiler.Syntax.Expressions;

namespace Sigil.Compiler.Semantics.TypedExpressions;

public sealed class TypedBinaryExpression(
    BinaryExpression expression,
    TypedExpression left,
    TypedExpression right,
    SigilType type)
    : TypedExpression(type)
{
    public BinaryExpression Expression { get; } = expression;
    public TypedExpression Left { get; } = left;
    public TypedExpression Right { get; } = right;
}
