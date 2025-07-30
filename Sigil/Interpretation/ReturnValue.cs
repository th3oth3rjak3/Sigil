namespace Sigil.Interpretation;
public class ReturnValue(object? value) : Exception
{
    public object? Value => value;
}