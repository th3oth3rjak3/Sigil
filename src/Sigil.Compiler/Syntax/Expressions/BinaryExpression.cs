using Sigil.Compiler.Syntax.Primitives;

namespace Sigil.Compiler.Syntax.Expressions;

public sealed class BinaryExpression(
    Expression left,
    TokenKind operatorKind,
    Expression right): Expression
{
    public Expression Left { get; } = left;
    public TokenKind OperatorKind { get; } = operatorKind;
    public Expression Right { get; } = right;
}
