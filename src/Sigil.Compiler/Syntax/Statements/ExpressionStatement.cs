using Sigil.Compiler.Syntax.Expressions;

namespace Sigil.Compiler.Syntax.Statements;

public sealed class ExpressionStatement(
    Expression expression): Statement
{
    public Expression Expression { get; } = expression;
}
