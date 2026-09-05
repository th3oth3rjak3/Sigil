namespace Sigil.Compiler.Syntax;

public sealed class FloatLiteralExpression(double value): Expression
{
    public double Value { get; } = value;
}
