namespace Sigil.Compiler.Semantics;

public sealed class Builtin(
    string name,
    IReadOnlyList<Type> parameterTypes,
    Type returnType,
    string runtimeName)
{
    public string Name { get; } = name;
    public IReadOnlyList<Type> ParameterTypes { get; } = parameterTypes;
    public Type ReturnType { get; } = returnType;
    public string RuntimeName { get; } = runtimeName;
}
