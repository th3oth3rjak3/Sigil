namespace Sigil.Compiler.Syntax;

public sealed class Module(IReadOnlyList<Declaration> declarations)
{
    public IReadOnlyList<Declaration> Declarations { get; } = declarations;
}
