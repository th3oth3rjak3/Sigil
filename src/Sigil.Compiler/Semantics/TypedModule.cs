namespace Sigil.Compiler.Semantics;

public sealed class TypedModule(
    IReadOnlyList<TypedFunctionDeclaration> declarations)
{
    public IReadOnlyList<TypedFunctionDeclaration> Declarations { get; }
        = declarations;
}
