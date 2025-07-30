using Sigil.ErrorHandling;
using Sigil.Lexing;
using Sigil.Parsing;
using Sigil.Parsing.Expressions;
using Sigil.Parsing.Statements;

namespace Sigil.Tests.Parsing;
public class ParserTests
{
    private Parser CreateParser(string source)
    {
        var errorHandler = new ErrorHandler(source);
        var lexer = new Lexer(source, errorHandler);
        var tokens = lexer.Tokenize();
        return new Parser(tokens, errorHandler, source);
    }

    [Theory]
    [InlineData("42")]
    [InlineData("3.14")]
    [InlineData("\"hello\"")]
    [InlineData("true")]
    [InlineData("false")]
    [InlineData("identifier")]
    public void Parser_ShouldParseLiteralExpressions(string input)
    {
        var source = $"return {input};";
        var parser = CreateParser(source);
        var statements = parser.Parse();

        Assert.Single(statements);
        Assert.IsType<ReturnStatement>(statements[0]);

        var exprStmt = (ReturnStatement)statements[0];
        Assert.NotNull(exprStmt.Expression);
    }

    [Fact]
    public void Parser_ShouldParseIntegerLiteral()
    {
        var parser = CreateParser("return 42;");
        var statements = parser.Parse();

        var exprStmt = (ReturnStatement)statements[0];
        var literal = Assert.IsType<IntegerLiteralExpression>(exprStmt.Expression);
        Assert.Equal(42, literal.Value);
    }

    [Fact]
    public void Parser_ShouldParseFloatLiteral()
    {
        var parser = CreateParser("return 3.14;");
        var statements = parser.Parse();

        var exprStmt = (ReturnStatement)statements[0];
        var literal = Assert.IsType<FloatLiteralExpression>(exprStmt.Expression);
        Assert.Equal(3.14, literal.Value);
    }

    [Fact]
    public void Parser_ShouldParseStringLiteral()
    {
        var parser = CreateParser("return \"hello world\";");
        var statements = parser.Parse();

        var exprStmt = (ReturnStatement)statements[0];
        var literal = Assert.IsType<StringLiteralExpression>(exprStmt.Expression);
        Assert.Equal("hello world", literal.Value);
    }

    [Fact]
    public void Parser_ShouldParseBooleanLiterals()
    {
        var parser = CreateParser("return true;");
        var statements = parser.Parse();

        var exprStmt = (ReturnStatement)statements[0];
        var literal = Assert.IsType<BooleanLiteralExpression>(exprStmt.Expression);
        Assert.True(literal.Value);
    }

    [Fact]
    public void Parser_ShouldParseIdentifier()
    {
        var parser = CreateParser("return myVariable;");
        var statements = parser.Parse();

        var exprStmt = (ReturnStatement)statements[0];
        var identifier = Assert.IsType<IdentifierExpression>(exprStmt.Expression);
        Assert.Equal("myVariable", identifier.Name);
    }

    [Fact]
    public void Parser_ShouldParseSimpleBinaryExpression()
    {
        var parser = CreateParser("return 1 + 2;");
        var statements = parser.Parse();

        var exprStmt = (ReturnStatement)statements[0];
        var binary = Assert.IsType<BinaryExpression>(exprStmt.Expression);

        Assert.IsType<IntegerLiteralExpression>(binary.Left);
        Assert.Equal(TokenType.Plus, binary.Operator.TokenType);
        Assert.IsType<IntegerLiteralExpression>(binary.Right);
    }

    [Fact]
    public void Parser_ShouldRespectOperatorPrecedence()
    {
        var parser = CreateParser("return 1 + 2 * 3;");
        var statements = parser.Parse();

        var exprStmt = (ReturnStatement)statements[0];
        var binary = Assert.IsType<BinaryExpression>(exprStmt.Expression);

        // Should be: 1 + (2 * 3)
        Assert.IsType<IntegerLiteralExpression>(binary.Left); // 1
        Assert.Equal(TokenType.Plus, binary.Operator.TokenType);
        Assert.IsType<BinaryExpression>(binary.Right); // (2 * 3)

        var rightBinary = (BinaryExpression)binary.Right;
        Assert.Equal(TokenType.Star, rightBinary.Operator.TokenType);
    }

    [Fact]
    public void Parser_ShouldParseUnaryExpression()
    {
        var parser = CreateParser("return -42;");
        var statements = parser.Parse();

        var exprStmt = (ReturnStatement)statements[0];
        var unary = Assert.IsType<UnaryExpression>(exprStmt.Expression);

        Assert.Equal(TokenType.Minus, unary.Operator.TokenType);
        Assert.IsType<IntegerLiteralExpression>(unary.Right);
    }

    [Fact]
    public void Parser_ShouldParseGroupingExpression()
    {
        var parser = CreateParser("return (1 + 2) * 3;");
        var statements = parser.Parse();

        var exprStmt = (ReturnStatement)statements[0];
        var binary = Assert.IsType<BinaryExpression>(exprStmt.Expression);

        // Should be: (1 + 2) * 3
        Assert.IsType<GroupingExpression>(binary.Left);
        Assert.Equal(TokenType.Star, binary.Operator.TokenType);
        Assert.IsType<IntegerLiteralExpression>(binary.Right);
    }

    [Fact]
    public void Parser_ShouldParseLetStatement()
    {
        var parser = CreateParser("let x = 42;");
        var statements = parser.Parse();

        Assert.Single(statements);
        var letStmt = Assert.IsType<LetStatement>(statements[0]);

        Assert.Equal("x", letStmt.Name);
        Assert.IsType<IntegerLiteralExpression>(letStmt.Initializer);
    }

    [Fact]
    public void Parser_ShouldParseLetStatementWithExpression()
    {
        var parser = CreateParser("let result = 1 + 2 * 3;");
        var statements = parser.Parse();

        var letStmt = Assert.IsType<LetStatement>(statements[0]);
        Assert.Equal("result", letStmt.Name);
        Assert.IsType<BinaryExpression>(letStmt.Initializer);
    }

    [Fact]
    public void Parser_ShouldParseBlockStatement()
    {
        var parser = CreateParser("{ let x = 1; let y = 2; }");
        var statements = parser.Parse();

        Assert.Single(statements);
        var block = Assert.IsType<BlockStatement>(statements[0]);
        Assert.Equal(2, block.Statements.Count);
        Assert.All(block.Statements, stmt => Assert.IsType<LetStatement>(stmt));
    }

    [Fact]
    public void Parser_ShouldParseMultipleStatements()
    {
        var source = """
        let x = 42;
        let y = x + 1;
        return y;
        """;

        var parser = CreateParser(source);
        var statements = parser.Parse();

        Assert.Equal(3, statements.Count);
        Assert.IsType<LetStatement>(statements[0]);
        Assert.IsType<LetStatement>(statements[1]);
        Assert.IsType<ReturnStatement>(statements[2]);
    }

    [Fact]
    public void Parser_ShouldHandleMissingSemicolon_AndReportError()
    {
        var source = "let x = 42";
        var errorHandler = new ErrorHandler(source);
        var lexer = new Lexer(source, errorHandler);
        var tokens = lexer.Tokenize();
        var parser = new Parser(tokens, errorHandler, source);

        var statements = parser.Parse();

        // Should still parse the let statement
        Assert.Single(statements);
        Assert.IsType<LetStatement>(statements[0]);

        // But should report an error
        Assert.True(errorHandler.HadError);
    }

    [Fact]
    public void Parser_ShouldHandleMissingInitializer_AndReportError()
    {
        var source = "let x;";
        var errorHandler = new ErrorHandler(source);
        var lexer = new Lexer(source, errorHandler);
        var tokens = lexer.Tokenize();
        var parser = new Parser(tokens, errorHandler, source);

        var statements = parser.Parse();

        // Should not parse successfully
        Assert.Empty(statements);
        Assert.True(errorHandler.HadError);
    }

    [Fact]
    public void Parser_ShouldHandleInvalidExpression_AndReportError()
    {
        var source = "let x = +;";
        var errorHandler = new ErrorHandler(source);
        var lexer = new Lexer(source, errorHandler);
        var tokens = lexer.Tokenize();
        var parser = new Parser(tokens, errorHandler, source);

        var statements = parser.Parse();

        Assert.Empty(statements);
        Assert.True(errorHandler.HadError);
    }

    [Fact]
    public void Parser_ShouldRecoverFromErrors()
    {
        var source = """
        let x = +;
        let y = 42;
        """;

        var errorHandler = new ErrorHandler(source);
        var lexer = new Lexer(source, errorHandler);
        var tokens = lexer.Tokenize();
        var parser = new Parser(tokens, errorHandler, source);

        var statements = parser.Parse();

        // Should parse the second statement even though first failed
        Assert.Single(statements);
        Assert.IsType<LetStatement>(statements[0]);

        var letStmt = (LetStatement)statements[0];
        Assert.Equal("y", letStmt.Name);

        Assert.True(errorHandler.HadError);
    }

    [Fact]
    public void Parser_ShouldHandleEmptyInput()
    {
        var parser = CreateParser("");
        var statements = parser.Parse();

        Assert.Empty(statements);
    }

    [Fact]
    public void Parser_ShouldHandleNestedBlocks()
    {
        var source = "{ { let x = 1; } let y = 2; }";
        var parser = CreateParser(source);
        var statements = parser.Parse();

        Assert.Single(statements);
        var outerBlock = Assert.IsType<BlockStatement>(statements[0]);
        Assert.Equal(2, outerBlock.Statements.Count);

        Assert.IsType<BlockStatement>(outerBlock.Statements[0]);
        Assert.IsType<LetStatement>(outerBlock.Statements[1]);
    }
}
