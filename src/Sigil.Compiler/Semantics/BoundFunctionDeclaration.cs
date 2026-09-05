namespace Sigil.Compiler.Semantics;

using Sigil.Compiler.Syntax;

public sealed class BoundFunctionDeclaration(
    FunctionDeclaration declaration,
    BoundBlock body)
{
    public FunctionDeclaration Declaration { get; } = declaration;

    public BoundBlock Body { get; } = body;
}
