using Sigil.Compiler.Syntax;

namespace Sigil.Compiler.Semantics;

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
