namespace Sigil.Compiler.Semantics;

using Sigil.Compiler.Syntax.Declarations;
using Sigil.Compiler.Syntax.Statements;

public sealed class TypedLetStatement(
    LetStatement declaration,
    VariableDeclaration variable,
    Type type,
    TypedExpression initializer)
    : TypedStatement
{
    public LetStatement Declaration { get; } = declaration;

    public VariableDeclaration Variable { get; } = variable;

    public Type Type { get; } = type;

    public TypedExpression Initializer { get; } = initializer;
}
