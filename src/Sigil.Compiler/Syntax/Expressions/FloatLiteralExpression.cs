namespace Sigil.Compiler.Syntax.Expressions;

public sealed class FloatLiteralExpression(double value): Expression
{
    public double Value { get; } = value;
}
