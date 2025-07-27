using Sigil.Common;

namespace Sigil.ErrorHandling;

/// <summary>
/// ErrorHandler collects error details throughout the compilation
/// process. If errors occur before the compiler backend takes over,
/// compilation progress will halt and all errors will be printed
/// to the terminal.
/// </summary>
/// <param name="sourceCode">The input source code used for displaying errors.</param>
public class ErrorHandler(string sourceCode)
{
    private readonly string _sourceCode = sourceCode;

    /// <summary>
    /// A list of all accumulate errors that will be printed if any errors occurred.
    /// </summary>
    public List<string> Errors { get; set; } = [];

    /// <summary>
    /// HadError is true if there are any errors.
    /// </summary>
    public bool HadError => Errors.Count > 0;

    /// <summary>
    /// Document that an error occurred in the compiler with its location
    /// and the context of the error.
    /// </summary>
    /// <param name="error">The details of the compilation error.</param>
    /// <param name="location">Where the error occurred in the source code.</param>
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
