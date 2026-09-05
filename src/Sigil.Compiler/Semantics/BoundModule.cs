namespace Sigil.Compiler.Semantics;

using Sigil.Compiler.Syntax;

public sealed class BoundModule(
    IReadOnlyList<BoundFunctionDeclaration> declarations)
{
    public IReadOnlyList<BoundFunctionDeclaration> Declarations { get; }
        = declarations;
}