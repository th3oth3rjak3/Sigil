using Sigil.Common;

namespace Sigil.Interpretation;

public class Environment
{
    private readonly Dictionary<string, object?> _values = new();
    private readonly Environment? _enclosing;

    public Environment()
    {
        _enclosing = null;
    }

    public Environment(Environment enclosing)
    {
        _enclosing = enclosing;
    }

    public void Define(string name, object? value)
    {
        _values[name] = value;
    }

    public object? Get(string name, Span span)
    {

        if (_values.TryGetValue(name, out var value))
        {
            return value;
        }

        if (_enclosing != null)
        {
            return _enclosing.Get(name, span);
        }

        throw new RuntimeException($"Undefined variable '{name}'", span);
    }

    public void Set(string name, object? value, Span span)
    {
        if (_values.ContainsKey(name))
        {
            _values[name] = value;
            return;
        }

        if (_enclosing != null)
        {
            _enclosing.Set(name, value, span);
            return;
        }

        throw new RuntimeException($"Undefined variable '{name}'", span);
    }
}
