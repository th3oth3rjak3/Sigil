namespace Sigil.Compiler.Semantics;

using Sigil.Compiler.Syntax;

public sealed class BoundIntegerLiteralExpression(IntegerLiteralExpression expression) : BoundExpression
{
    public IntegerLiteralExpression Expression { get; } = expression;
}