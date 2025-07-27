using Sigil.CodeGeneration;
using Sigil.ErrorHandling;

namespace Sigil;

public class Compiler(string SourceCode, ICompilerBackend Backend)
{
    private ErrorHandler _errorHandler = new(SourceCode);



    // TODO: this should return a "Program" or a "Statement List"
    public int Compile()
    {
        // TODO: actually compile the code, then execute the AST
        return Backend.Execute([]);
    }
}
