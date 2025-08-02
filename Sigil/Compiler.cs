using Sigil.CodeGeneration;
using Sigil.ErrorHandling;
using Sigil.Lexing;
using Sigil.Parsing;
using Sigil.TypeChecking;

namespace Sigil;

public class Compiler(string SourceCode, ICompilerBackend Backend)
{
    private ErrorHandler _errorHandler = new(SourceCode);

    public int Compile()
    {
        var lexer = new Lexer(SourceCode, _errorHandler);
        var tokens = lexer.Tokenize();
        var parser = new Parser(tokens, _errorHandler, SourceCode);
        var ast = parser.Parse();
        var typeChecker = new TypeCheckingVisitor(_errorHandler);
        typeChecker.TypeCheck(ast);

        // Check if we had errors
        if (_errorHandler.HadError)
        {
            foreach (var error in _errorHandler.Errors)
            {
                Console.WriteLine(error);
            }

            return 1;
        }

        return Backend.Execute(ast);
    }
}
