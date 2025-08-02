using Sigil.CodeGeneration;
using Sigil.Parsing.Expressions;
using Sigil.Parsing.Statements;
using System.Text;

namespace Sigil.Debugging;

public class AstPrintingVisitor : IStatementVisitor<string>, IExpressionVisitor<string>, ICompilerBackend
{
    private int indentLevel = 0;
    private string Indent() => new(' ', indentLevel * 2);

    public string VisitAssignmentStatement(AssignmentStatement statement)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"{Indent()}AssignmentStatement {{");
        indentLevel++;
        sb.AppendLine($"{Indent()}Name: Identifier \"{statement.Name}\",");
        sb.AppendLine($"{Indent()}Value: ");
        indentLevel++;
        sb.AppendLine(statement.Value.Accept(this) + ",");
        indentLevel--;
        indentLevel--;
        sb.AppendLine($"{Indent()}}}");
        return sb.ToString();
    }

    public string VisitBinaryExpression(BinaryExpression expression)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"{Indent()}BinaryExpression {{");
        indentLevel++;
        // Handle the case where Operator might be a Token object
        string operatorStr = expression.Operator?.ToString() ?? "null";
        if (operatorStr.StartsWith("Token { TokenType = "))
        {
            // Extract just the token type from the Token object string representation
            var start = operatorStr.IndexOf("TokenType = ") + "TokenType = ".Length;
            var end = operatorStr.IndexOf(",", start);
            if (end == -1) end = operatorStr.IndexOf(" }", start);
            operatorStr = end > start ? operatorStr.Substring(start, end - start) : operatorStr;
        }

        sb.AppendLine($"{Indent()}Left: ");
        indentLevel++;
        sb.AppendLine(expression.Left.Accept(this) + ",");
        indentLevel--;
        sb.AppendLine($"{Indent()}Operator: \"{operatorStr}\",");
        sb.AppendLine($"{Indent()}Right: ");
        indentLevel++;
        sb.AppendLine(expression.Right.Accept(this) + ",");
        indentLevel--;
        indentLevel--;
        sb.Append($"{Indent()}}}");
        return sb.ToString();
    }

    public string VisitBlockStatement(BlockStatement statement)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"{Indent()}BlockStatement {{");
        indentLevel++;
        sb.AppendLine($"{Indent()}Statements: [");
        indentLevel++;
        foreach (var stmt in statement.Statements)
        {
            sb.AppendLine(stmt.Accept(this) + ",");
        }
        indentLevel--;
        sb.AppendLine($"{Indent()}],");
        indentLevel--;
        sb.Append($"{Indent()}}}");
        return sb.ToString();
    }

    public string VisitBooleanLiteralExpression(BooleanLiteralExpression expression)
    {
        return $"{Indent()}BooleanLiteral {{ Value: {expression.Value.ToString().ToLower()} }}";
    }

    public string VisitCallExpression(CallExpression expression)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"{Indent()}CallExpression {{");
        indentLevel++;
        sb.AppendLine($"{Indent()}Callee: ");
        indentLevel++;
        sb.AppendLine(expression.Callee.Accept(this) + ",");
        indentLevel--;
        sb.AppendLine($"{Indent()}Arguments: [");
        indentLevel++;
        foreach (var arg in expression.Arguments)
        {
            sb.AppendLine(arg.Accept(this) + ",");
        }
        indentLevel--;
        sb.AppendLine($"{Indent()}],");
        indentLevel--;
        sb.Append($"{Indent()}}}");
        return sb.ToString();
    }

    public string VisitCharacterLiteralExpression(CharacterLiteralExpression expression)
    {
        return $"{Indent()}CharacterLiteral {{ Value: '{expression.Value}' }}";
    }

    public string VisitClassStatement(ClassStatement statement)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"{Indent()}ClassStatement {{");
        indentLevel++;
        sb.AppendLine($"{Indent()}Name: Identifier \"{statement.Name}\",");

        if (!string.IsNullOrEmpty(statement.SuperclassName))
        {
            sb.AppendLine($"{Indent()}Superclass: Identifier \"{statement.SuperclassName}\",");
        }

        sb.AppendLine($"{Indent()}Fields: [");
        indentLevel++;
        foreach (var field in statement.Fields)
        {
            sb.AppendLine($"{Indent()}FieldDeclaration {{ Name: \"{field.Name}\", Type: \"{field.TypeName}\" }},");
        }
        indentLevel--;
        sb.AppendLine($"{Indent()}],");

        sb.AppendLine($"{Indent()}Methods: [");
        indentLevel++;
        foreach (var method in statement.Methods)
        {
            sb.AppendLine(method.Accept(this) + ",");
        }
        indentLevel--;
        sb.AppendLine($"{Indent()}],");

        indentLevel--;
        sb.AppendLine($"{Indent()}}}");
        return sb.ToString();
    }

    public string VisitExpressionStatement(ExpressionStatement statement)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"{Indent()}ExpressionStatement {{");
        indentLevel++;
        sb.AppendLine($"{Indent()}Expression: ");
        indentLevel++;
        sb.AppendLine(statement.Expression.Accept(this) + ",");
        indentLevel--;
        indentLevel--;
        sb.Append($"{Indent()}}}");
        return sb.ToString();
    }

    public string VisitFloatLiteralExpression(FloatLiteralExpression expression)
    {
        return $"{Indent()}FloatLiteral {{ Value: {expression.Value} }}";
    }

    public string VisitFunctionStatement(FunctionStatement statement)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"{Indent()}FunctionDeclaration {{");
        indentLevel++;
        sb.AppendLine($"{Indent()}Name: Identifier \"{statement.Name}\",");
        sb.AppendLine($"{Indent()}Parameters: [");
        indentLevel++;
        foreach (var param in statement.Parameters)
        {
            sb.AppendLine($"{Indent()}Identifier \"{param}\",");
        }
        indentLevel--;
        sb.AppendLine($"{Indent()}],");
        sb.AppendLine($"{Indent()}Body: [");
        indentLevel++;
        foreach (var stmt in statement.Body)
        {
            sb.AppendLine(stmt.Accept(this) + ",");
        }
        indentLevel--;
        sb.AppendLine($"{Indent()}],");
        indentLevel--;
        sb.Append($"{Indent()}}}");
        return sb.ToString();
    }

    public string VisitGroupingExpression(GroupingExpression expression)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"{Indent()}GroupingExpression {{");
        indentLevel++;
        sb.AppendLine($"{Indent()}Expression: ");
        indentLevel++;
        sb.AppendLine(expression.Expression.Accept(this) + ",");
        indentLevel--;
        indentLevel--;
        sb.Append($"{Indent()}}}");
        return sb.ToString();
    }

    public string VisitIdentifierExpression(IdentifierExpression expression)
    {
        return $"{Indent()}Identifier \"{expression.Name}\"";
    }

    public string VisitIfStatement(IfStatement statement)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"{Indent()}IfStatement {{");
        indentLevel++;
        sb.AppendLine($"{Indent()}Condition: ");
        indentLevel++;
        sb.AppendLine(statement.Condition.Accept(this) + ",");
        indentLevel--;
        sb.AppendLine($"{Indent()}ThenBranch: ");
        indentLevel++;
        sb.AppendLine(statement.ThenBranch.Accept(this) + ",");
        indentLevel--;
        if (statement.ElseBranch is not null)
        {
            sb.AppendLine($"{Indent()}ElseBranch: ");
            indentLevel++;
            sb.AppendLine(statement.ElseBranch.Accept(this) + ",");
            indentLevel--;
        }
        indentLevel--;
        sb.Append($"{Indent()}}}");
        return sb.ToString();
    }

    public string VisitIntegerLiteralExpression(IntegerLiteralExpression expression)
    {
        return $"{Indent()}IntegerLiteral {{ Value: {expression.Value} }}";
    }

    public string VisitLetStatement(LetStatement statement)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"{Indent()}LetStatement {{");
        indentLevel++;
        sb.AppendLine($"{Indent()}Name: Identifier \"{statement.Name}\",");
        sb.AppendLine($"{Indent()}Initializer: ");
        indentLevel++;
        sb.AppendLine(statement.Initializer.Accept(this) + ",");
        indentLevel--;
        indentLevel--;
        sb.Append($"{Indent()}}}");
        return sb.ToString();
    }

    public string VisitReturnStatement(ReturnStatement statement)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"{Indent()}ReturnStatement {{");
        indentLevel++;
        if (statement.Expression is not null)
        {
            sb.AppendLine($"{Indent()}Expression: ");
            indentLevel++;
            sb.AppendLine(statement.Expression.Accept(this) + ",");
            indentLevel--;
        }
        else
        {
            sb.AppendLine($"{Indent()}Expression: null,");
        }
        indentLevel--;
        sb.Append($"{Indent()}}}");
        return sb.ToString();
    }

    public string VisitStringLiteralExpression(StringLiteralExpression expression)
    {
        return $"{Indent()}StringLiteral {{ Value: \"{expression.Value}\" }}";
    }

    public string VisitUnaryExpression(UnaryExpression expression)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"{Indent()}UnaryExpression {{");
        indentLevel++;

        // Handle the case where Operator might be a Token object
        string operatorStr = expression.Operator?.ToString() ?? "null";
        if (operatorStr.StartsWith("Token { TokenType = "))
        {
            // Extract just the token type from the Token object string representation
            var start = operatorStr.IndexOf("TokenType = ") + "TokenType = ".Length;
            var end = operatorStr.IndexOf(",", start);
            if (end == -1) end = operatorStr.IndexOf(" }", start);
            operatorStr = end > start ? operatorStr.Substring(start, end - start) : operatorStr;
        }

        sb.AppendLine($"{Indent()}Operator: \"{operatorStr}\",");
        sb.AppendLine($"{Indent()}Right: ");
        indentLevel++;
        sb.AppendLine(expression.Right.Accept(this) + ",");
        indentLevel--;
        indentLevel--;
        sb.Append($"{Indent()}}}");
        return sb.ToString();
    }

    public string VisitWhileStatement(WhileStatement statement)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"{Indent()}WhileStatement {{");
        indentLevel++;
        sb.AppendLine($"{Indent()}Condition: ");
        indentLevel++;
        sb.AppendLine(statement.Condition.Accept(this) + ",");
        indentLevel--;
        sb.AppendLine($"{Indent()}Body: ");
        indentLevel++;
        sb.AppendLine(statement.Body.Accept(this) + ",");
        indentLevel--;
        indentLevel--;
        sb.Append($"{Indent()}}}");
        return sb.ToString();
    }

    public int Execute(List<Statement> nodes)
    {
        // For printing, just output the whole AST to console.
        foreach (var node in nodes)
        {
            Console.WriteLine(node.Accept(this));
        }
        return 0;
    }
}
