using Sigil.Compiler.Semantics.BoundPrimitives;
using Sigil.Compiler.Syntax.Declarations;

namespace Sigil.Compiler.Semantics.BoundDeclarations;

public sealed class BoundFunctionDeclaration(
    FunctionDeclaration declaration,
    BoundBlock body)
{
    public FunctionDeclaration Declaration { get; } = declaration;

    public BoundBlock Body { get; } = body;
}
