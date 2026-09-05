namespace Sigil.Compiler.Semantics;

using Sigil.Compiler.Syntax.Declarations;

public sealed class TypedFunctionDeclaration(
    FunctionDeclaration declaration,
    Type returnType,
    TypedBlock body)
{
    public FunctionDeclaration Declaration { get; } = declaration;

    public Type ReturnType { get; } = returnType;

    public TypedBlock Body { get; } = body;
}
