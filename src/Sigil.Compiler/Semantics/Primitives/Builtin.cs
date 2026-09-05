using Sigil.Compiler.Semantics.Types;

namespace Sigil.Compiler.Semantics.Primitives;

public sealed class Builtin(
    string name,
    IReadOnlyList<SigilType> parameterTypes,
    SigilType returnType,
    string runtimeName)
{
    public string Name { get; } = name;
    public IReadOnlyList<SigilType> ParameterTypes { get; } = parameterTypes;
    public SigilType ReturnType { get; } = returnType;
    public string RuntimeName { get; } = runtimeName;
}
