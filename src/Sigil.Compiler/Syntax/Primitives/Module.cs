using Sigil.Compiler.Syntax.Declarations;

namespace Sigil.Compiler.Syntax.Primitives;

public sealed class Module(IReadOnlyList<Declaration> declarations)
{
    public IReadOnlyList<Declaration> Declarations { get; } = declarations;
}
