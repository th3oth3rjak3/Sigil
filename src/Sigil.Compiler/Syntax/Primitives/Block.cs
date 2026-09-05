using Sigil.Compiler.Syntax.Statements;

namespace Sigil.Compiler.Syntax.Primitives;

public sealed class Block(IReadOnlyList<Statement> statements)
{
    public IReadOnlyList<Statement> Statements { get; } = statements;
}
