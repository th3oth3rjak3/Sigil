namespace Sigil.Compiler.Semantics;

using Sigil.Compiler.Syntax;

public sealed class TypedReturnStatement(
    ReturnStatement statement,
    TypedExpression? value)
    : TypedStatement
{
    public ReturnStatement Statement { get; } = statement;
    public TypedExpression? Value { get; } = value;
}
