using Sigil.Common;

namespace Sigil.ErrorHandling;

public class ErrorHandler
{
    private List<string> _errors = [];

    public bool HadError => _errors.Count > 0;

    public void Report(string error, Position location)
    {
        _errors.Add($"[{location.Line}:{location.Column}] Error: {error}");
    }
}
