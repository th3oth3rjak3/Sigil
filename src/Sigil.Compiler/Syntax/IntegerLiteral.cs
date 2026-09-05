namespace Sigil.Compiler.Syntax;

public sealed class IntegerLiteralExpression(long value): Expression
{
    public long Value { get; } = value;
}
