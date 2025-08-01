using Sigil.Parsing;
using Sigil.Parsing.Statements;

namespace Sigil.CodeGeneration;


public interface ICompilerBackend
{
    public int Execute(List<Statement> nodes);
}
