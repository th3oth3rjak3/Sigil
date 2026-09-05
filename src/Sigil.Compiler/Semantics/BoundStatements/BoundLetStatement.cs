using Sigil.Compiler.Semantics.BoundExpressions;
using Sigil.Compiler.Syntax.Declarations;
using Sigil.Compiler.Syntax.Statements;

namespace Sigil.Compiler.Semantics.BoundStatements;

public sealed class BoundLetStatement(
    LetStatement declaration,
    VariableDeclaration variable,
    BoundExpression initializer)
    : BoundStatement
{
    public LetStatement Declaration { get; } = declaration;
    public VariableDeclaration Variable { get; } = variable;
    public BoundExpression Initializer { get; } = initializer;
}
