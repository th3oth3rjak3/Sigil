namespace Sigil.Compiler.Semantics;

using Sigil.Compiler.Syntax.Statements;

public sealed class BoundReturnStatement(ReturnStatement statement, BoundExpression? value): BoundStatement
{
    public ReturnStatement Statement { get; } = statement;
    public BoundExpression? Value { get; } = value;
}
