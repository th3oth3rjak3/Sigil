using Sigil.Compiler.Semantics.TypedDeclarations;

namespace Sigil.Compiler.Semantics.TypedPrimitives;

public sealed class TypedModule(
    IReadOnlyList<TypedFunctionDeclaration> declarations)
{
    public IReadOnlyList<TypedFunctionDeclaration> Declarations { get; } = declarations;
}
