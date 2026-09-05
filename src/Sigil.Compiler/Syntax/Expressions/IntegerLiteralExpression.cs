namespace Sigil.Compiler.Syntax.Expressions;

public sealed class IntegerLiteralExpression(long value): Expression
{
    public long Value { get; } = value;
}
