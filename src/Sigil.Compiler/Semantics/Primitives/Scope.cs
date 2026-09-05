namespace Sigil.Compiler.Semantics.Primitives;

internal sealed class Scope(Scope? parent = null)
{
    private readonly Dictionary<string, Symbol> _symbols = [];

    public Scope? Parent { get; } = parent;

    public void Declare(Symbol symbol)
    {
        if (!_symbols.TryAdd(symbol.Name, symbol))
        {
            throw new Exception(
                $"The name '{symbol.Name}' is already declared in this scope.");
        }
    }

    public Symbol Resolve(string name)
    {
        if (_symbols.TryGetValue(name, out var symbol))
        {
            return symbol;
        }

        if (Parent is not null)
        {
            return Parent.Resolve(name);
        }

        throw new Exception($"The name '{name}' could not be resolved.");
    }
}
