namespace Sigil.Compiler.Syntax;

public sealed class ReturnStatement(Expression? value): Statement
{
    public Expression? Value { get; } = value;
}
