namespace Sigil.Parsing.Expressions;
public interface IExpressionVisitor<T>
{
    public T VisitBinaryExpression(BinaryExpression BinaryExpression);
    public T VisitUnaryExpression(UnaryExpression UnaryExpression);
    public T VisitGroupingExpression(GroupingExpression GroupingExpression);
    public T VisitFloatLiteralExpression(FloatLiteralExpression FloatLiteralExpression);
    public T VisitIntegerLiteralExpression(IntegerLiteralExpression IntegerLiteralExpression);
    public T VisitStringLiteralExpression(StringLiteralExpression StringLiteralExpression);
    public T VisitCharacterLiteralExpression(CharacterLiteralExpression CharacterLiteralExpression);
    public T VisitBooleanLiteralExpression(BooleanLiteralExpression BooleanLiteralExpression);
}
