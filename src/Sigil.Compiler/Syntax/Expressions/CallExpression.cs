namespace Sigil.Compiler.Syntax.Expressions;

public sealed class CallExpression(
    Expression callee,
    IReadOnlyList<Expression> arguments): Expression
{
    public Expression Callee { get; } = callee;
    public IReadOnlyList<Expression> Arguments { get; } = arguments;
}
