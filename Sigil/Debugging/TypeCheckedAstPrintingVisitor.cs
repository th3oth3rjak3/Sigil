using Sigil.CodeGeneration;
using Sigil.ErrorHandling;
using Sigil.Parsing.Expressions;
using Sigil.Parsing.Statements;
using Sigil.TypeChecking;

namespace Sigil.Debugging;

public class TypeCheckedAstPrintingVisitor(ErrorHandler ErrorHandler) : AstPrintingVisitor, IStatementVisitor<string>, IExpressionVisitor<string>, ICompilerBackend
{
    public new int Execute(List<Statement> nodes)
    {
        var typeChecker = new TypeCheckingVisitor(ErrorHandler);
        typeChecker.TypeCheck(nodes);

        foreach (var node in nodes)
        {
            Console.WriteLine(node.Accept(this));
        }

        return 0;
    }
}
