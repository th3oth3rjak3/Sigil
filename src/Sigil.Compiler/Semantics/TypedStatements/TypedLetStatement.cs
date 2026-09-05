using Sigil.Compiler.Semantics.TypedExpressions;
using Sigil.Compiler.Semantics.Types;
using Sigil.Compiler.Syntax.Declarations;
using Sigil.Compiler.Syntax.Statements;

namespace Sigil.Compiler.Semantics.TypedStatements;

public sealed class TypedLetStatement(
    LetStatement declaration,
    VariableDeclaration variable,
    SigilType type,
    TypedExpression initializer)
    : TypedStatement
{
    public LetStatement Declaration { get; } = declaration;

    public VariableDeclaration Variable { get; } = variable;

    public SigilType Type { get; } = type;

    public TypedExpression Initializer { get; } = initializer;
}
