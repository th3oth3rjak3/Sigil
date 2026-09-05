namespace Sigil.Compiler.Syntax;

public sealed class Block(IReadOnlyList<Statement> statements)
{
    public IReadOnlyList<Statement> Statements { get; } = statements;
}