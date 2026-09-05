namespace Sigil.Compiler.Syntax;

public sealed class FunctionDeclaration(
    string name,
    IReadOnlyList<string> parameters,
    Block body) : Declaration
{
    public string Name { get; } = name;
    public IReadOnlyList<string> Parameters { get; } = parameters;
    public Block Body { get; } = body;
}