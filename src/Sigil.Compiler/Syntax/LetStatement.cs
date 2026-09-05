namespace Sigil.Compiler.Syntax;

public sealed class LetStatement(
    string name,
    string type,
    Expression initializer): Statement
{
    public string Name { get; } = name;
    public string Type { get; } = type;
    public Expression Initializer { get; } = initializer;
}
