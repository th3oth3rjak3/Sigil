namespace Sigil.Compiler.Syntax;

public sealed class BinaryExpression(
    Expression left,
    TokenKind operatorKind,
    Expression right) : Expression
{
    public Expression Left { get; } = left;
    public TokenKind OperatorKind { get; } = operatorKind;
    public Expression Right { get; } = right;
}