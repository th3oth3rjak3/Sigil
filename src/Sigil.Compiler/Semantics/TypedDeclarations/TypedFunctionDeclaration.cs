namespace Sigil.Compiler.Semantics.TypedDeclarations;

using Sigil.Compiler.Semantics.TypedPrimitives;
using Sigil.Compiler.Semantics.Types;
using Sigil.Compiler.Syntax.Declarations;

public sealed class TypedFunctionDeclaration(
    FunctionDeclaration declaration,
    SigilType returnType,
    TypedBlock body)
{
    public FunctionDeclaration Declaration { get; } = declaration;

    public SigilType ReturnType { get; } = returnType;

    public TypedBlock Body { get; } = body;
}
