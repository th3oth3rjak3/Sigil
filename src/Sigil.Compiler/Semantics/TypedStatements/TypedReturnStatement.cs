using Sigil.Compiler.Semantics.TypedExpressions;
using Sigil.Compiler.Syntax.Statements;

namespace Sigil.Compiler.Semantics.TypedStatements;

public sealed class TypedReturnStatement(
    ReturnStatement statement,
    TypedExpression? value)
    : TypedStatement
{
    public ReturnStatement Statement { get; } = statement;
    public TypedExpression? Value { get; } = value;
}
