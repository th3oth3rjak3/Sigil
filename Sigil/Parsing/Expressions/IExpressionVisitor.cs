namespace Sigil.Parsing.Expressions;
public interface IExpressionVisitor<T>
{
    public T VisitBinaryExpression(BinaryExpression expression);
    public T VisitUnaryExpression(UnaryExpression expression);
    public T VisitGroupingExpression(GroupingExpression expression);
    public T VisitFloatLiteralExpression(FloatLiteralExpression expression);
    public T VisitIntegerLiteralExpression(IntegerLiteralExpression expression);
    public T VisitStringLiteralExpression(StringLiteralExpression expression);
    public T VisitCharacterLiteralExpression(CharacterLiteralExpression expression);
    public T VisitBooleanLiteralExpression(BooleanLiteralExpression expression);
    public T VisitIdentifierExpression(IdentifierExpression expression);
    public T VisitCallExpression(CallExpression expression);
}
