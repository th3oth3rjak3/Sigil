namespace Sigil.Compiler.Syntax.Declarations;

public sealed class VariableDeclaration(string name, string type): Declaration
{
    public string Name { get; } = name;
    public string Type { get; } = type;
}
