namespace Sigil.Compiler.Syntax;

public class Parser(Lexer lexer)
{
    private Token _current;

    public Module Parse()
    {
        var declarations = new List<Declaration>();

        _current = lexer.NextToken();

        if (_current.Kind == TokenKind.Fn)
        {
            Advance();
            declarations.Add(ParseFunction());
        }

        return new Module(declarations);
    }

    private void Advance()
    {
        _current = lexer.NextToken();
    }

    private FunctionDeclaration ParseFunction()
    {
        var name = Expect(TokenKind.Identifier);

        Expect(TokenKind.LeftParen);

        var parameters = ParseParameters();

        Expect(TokenKind.RightParen);
        Expect(TokenKind.Arrow);

        var returnType = Expect(TokenKind.Identifier);

        Expect(TokenKind.LeftBrace);

        var body = ParseBlock();

        Expect(TokenKind.RightBrace);

        return new FunctionDeclaration(
            name.Lexeme,
            parameters,
            returnType.Lexeme,
            body);
    }

    private List<string> ParseParameters()
    {
        return [];
    }

    private Block ParseBlock()
    {
        var statements = new List<Statement>();

        while (_current.Kind != TokenKind.RightBrace)
        {
            statements.Add(ParseStatement());
        }

        return new Block(statements);
    }

    private Statement ParseStatement()
    {
        return _current.Kind switch
        {
            TokenKind.Return => ParseReturnStatement(),
            _ => throw new Exception(
                $"Unexpected token {_current.Kind} at position {_current.Position}.")
        };
    }

    private ReturnStatement ParseReturnStatement()
    {
        Expect(TokenKind.Return);

        Expression? value = null;

        if (_current.Kind != TokenKind.Semicolon)
        {
            var integer = Expect(TokenKind.IntegerLiteral);

            value = new IntegerLiteralExpression(long.Parse(integer.Lexeme));
        }

        Expect(TokenKind.Semicolon);

        return new ReturnStatement(value);
    }
    private Token Expect(TokenKind kind)
    {
        if (_current.Kind != kind)
        {
            throw new Exception(
                $"Expected {kind}, but found {_current.Kind} at position {_current.Position}.");
        }


        var token = _current;
        Advance();

        return token;
    }
}