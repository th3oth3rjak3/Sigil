namespace Sigil.Compiler.Syntax;

public sealed class IdentifierExpression(string name) : Expression
{
    public string Name { get; } = name;
}