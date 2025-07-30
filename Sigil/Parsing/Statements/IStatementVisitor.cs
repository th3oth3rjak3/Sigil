namespace Sigil.Parsing.Statements;
public interface IStatementVisitor<T>
{
    public T VisitLetStatement(LetStatement statement);
    public T VisitBlockStatement(BlockStatement statement);
    public T VisitExpressionStatement(ExpressionStatement statement);
    public T VisitReturnStatement(ReturnStatement statement);
    public T VisitIfStatement(IfStatement statement);
    public T VisitWhileStatement(WhileStatement statement);
    public T VisitAssignmentStatement(AssignmentStatement statement);
    public T VisitFunctionStatement(FunctionStatement statement);
}
