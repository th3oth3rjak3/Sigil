using Sigil.Compiler.Syntax.Expressions;

namespace Sigil.Compiler.Syntax.Statements;

public sealed class ReturnStatement(Expression? value): Statement
{
    public Expression? Value { get; } = value;
}
