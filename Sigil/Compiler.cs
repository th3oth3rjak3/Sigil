using Sigil.ErrorHandling;

namespace Sigil;

public class Compiler(string SourceCode)
{
    private ErrorHandler _errorHandler = new();

    public Result<string, Exception> Compile()
    {
        return Ok(SourceCode);
    }
}
