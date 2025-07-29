using Sigil.ErrorHandling;
using Sigil.Parsing.Statements;

namespace Sigil.Parsing;
public record UntypedProgram(List<Statement> Statements, ErrorHandler ErrorHandler)
{
}
