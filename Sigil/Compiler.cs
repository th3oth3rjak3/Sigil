using Sigil.ErrorHandling;

namespace Sigil;

public class Compiler(string SourceCode)
{
    private ErrorHandler _errorHandler = new(SourceCode);

    public Result<string, Exception> Compile()
    {
        return Ok(SourceCode);
    }
}
