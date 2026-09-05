namespace Sigil.Compiler.Semantics;

public sealed class TypedCallExpression(
    BoundCallExpression expression,
    Type type,
    IReadOnlyList<TypedExpression> arguments)
    : TypedExpression(type)
{
    public BoundCallExpression Expression { get; } = expression;
    public IReadOnlyList<TypedExpression> Arguments { get; } = arguments;
}
