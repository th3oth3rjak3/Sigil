namespace Sigil.Compiler.Syntax.Expressions;

public sealed class IdentifierExpression(string name): Expression
{
    public string Name { get; } = name;
}
