using Sigil.Compiler.Semantics.BoundExpressions;
using Sigil.Compiler.Syntax.Statements;

namespace Sigil.Compiler.Semantics.BoundStatements;

public sealed class BoundReturnStatement(ReturnStatement statement, BoundExpression? value): BoundStatement
{
    public ReturnStatement Statement { get; } = statement;
    public BoundExpression? Value { get; } = value;
}
