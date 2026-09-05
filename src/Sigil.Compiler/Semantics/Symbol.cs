namespace Sigil.Compiler.Semantics;

using Sigil.Compiler.Syntax.Declarations;

public sealed class Symbol(string name, Declaration declaration)
{
    public string Name { get; } = name;
    public Declaration Declaration { get; } = declaration;
}
