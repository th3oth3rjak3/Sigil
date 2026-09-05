namespace Sigil.Compiler.Semantics.BoundPrimitives;

using Sigil.Compiler.Semantics.BoundDeclarations;

public sealed class BoundModule(
    IReadOnlyList<BoundFunctionDeclaration> declarations)
{
    public IReadOnlyList<BoundFunctionDeclaration> Declarations { get; }
        = declarations;
}
