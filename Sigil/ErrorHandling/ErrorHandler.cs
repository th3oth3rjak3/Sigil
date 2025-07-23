using Sigil.Common;

namespace Sigil.ErrorHandling;

public class ErrorHandler(string sourceCode)
{
    private readonly string _sourceCode = sourceCode;

    public List<string> Errors { get; set; } = [];

    public bool HadError => Errors.Count > 0;

    public void Report(string error, Span location)
    {
        Errors.Add($"[{location.Start.Line}:{location.Start.Column}] Error: {error}");

        var lineOfOffendingCode = string.Join(
            "",
            _sourceCode
            .Skip(location.Start.LineOffset)
            .TakeWhile(ch => ch != '\n')
            .ToArray());

        var lineNumber = location.Start.Line.ToString();
        lineOfOffendingCode = $"{lineNumber} | {lineOfOffendingCode}";

        // Handle zero length spans to fail gracefully.
        var underlineLength = Math.Max(0, location.End.Column - location.Start.Column + 1);
        var pointer = new string(' ', lineNumber.Length + 3 + location.Start.Offset - location.Start.LineOffset)
                   + new string('^', underlineLength)
                   + " <- Error Here";

        Errors.Add(lineOfOffendingCode);
        Errors.Add(pointer);
    }
}
