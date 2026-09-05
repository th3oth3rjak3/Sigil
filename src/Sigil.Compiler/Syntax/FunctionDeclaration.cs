namespace Sigil.Compiler.Syntax;

public sealed class FunctionDeclaration(
    string name,
    IReadOnlyList<string> parameters,
    string returnType,
    Block body) : Declaration
{
    public string Name { get; } = name;
    public IReadOnlyList<string> Parameters { get; } = parameters;
    public string ReturnType { get; } = returnType;
    public Block Body { get; } = body;
}