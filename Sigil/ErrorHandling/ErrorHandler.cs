using Sigil.Common;

namespace Sigil.ErrorHandling;

/// <summary>
/// ErrorHandler collects error details throughout the compilation
/// process. If errors occur before the compiler backend takes over,
/// compilation progress will halt and all errors will be printed
/// to the terminal.
/// </summary>
/// <param name="sourceCode">The input source code used for displaying errors.</param>
public class ErrorHandler(string SourceCode)
{
    /// <summary>
    /// MAX_ERROR_DISPLAY_COUNT is the maximum number of errors to show to a user at a time
    /// to keep them from getting overwhelmed.
    /// </summary>
    const int MAX_ERROR_DISPLAY_COUNT = 5;

    private int _errorCount = 0;

    /// <summary>
    /// _suppressedErrorCount collects the number of errors that would have been reported,
    /// but were supressed because there were too many to display.
    /// </summary>
    private int _supressedErrorCount = 0;

    private bool _tooManyErrors => _errorCount >= MAX_ERROR_DISPLAY_COUNT;
    public int ErrorCount => _errorCount + _supressedErrorCount;

    private List<string> _errors = new();

    /// <summary>
    /// A list of all accumulate errors that will be printed if any errors occurred.
    /// </summary>
    public List<string> Errors => GetErrors();

    /// <summary>
    /// HadError is true if there are any errors.
    /// </summary>
    public bool HadError => ErrorCount > 0;

    /// <summary>
    /// Document that an error occurred in the compiler with its location
    /// and the context of the error.
    /// </summary>
    /// <param name="error">The details of the compilation error.</param>
    /// <param name="location">Where the error occurred in the source code.</param>
    public void Report(string error, Span location)
    {
        if (_tooManyErrors)
        {
            _supressedErrorCount++;
            return;
        }

        AddError(error, location);
    }

    private void AddError(string error, Span location)
    {
        _errors.Add($"[{location.Start.Line}:{location.Start.Column}] Error: {error}");

        var lineOfOffendingCode = string.Join(
            "",
            SourceCode
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

        _errors.Add(lineOfOffendingCode);
        _errors.Add(pointer);
        _errors.Add("\n");

        _errorCount++;
    }

    private List<string> GetErrors()
    {
        if (_tooManyErrors)
        {
            // Add the error suppression statistics
            var errorsToReport = _errors;
            errorsToReport.Add($"Showing {MAX_ERROR_DISPLAY_COUNT} of {ErrorCount} errors.");
            errorsToReport.Add("Fix the above errors and recompile to see the rest.");
            errorsToReport.Add("\n");
            return errorsToReport;
        }

        return _errors;
    }
}
