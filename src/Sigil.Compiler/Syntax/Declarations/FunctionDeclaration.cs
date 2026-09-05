using Sigil.Compiler.Syntax.Primitives;

namespace Sigil.Compiler.Syntax.Declarations;

public sealed class FunctionDeclaration(
    string name,
    IReadOnlyList<Parameter> parameters,
    string returnType,
    Block body): Declaration
{
    public string Name { get; } = name;
    public IReadOnlyList<Parameter> Parameters { get; } = parameters;
    public string ReturnType { get; } = returnType;
    public Block Body { get; } = body;
}
