using Sigil.Parsing;

namespace Sigil.CodeGeneration;


public interface ICompilerBackend
{
    public int Execute(List<AstNode> nodes);
}
