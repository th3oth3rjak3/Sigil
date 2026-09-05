namespace Sigil.Compiler.Syntax;

public sealed class ExpressionStatement(
    Expression expression): Statement
{
    public Expression Expression { get; } = expression;
}
