namespace Sigil.Compiler.Syntax;

public class Parser(Lexer lexer)
{
    private Token _current;

    public Module Parse()
    {
        var declarations = new List<Declaration>();

        _current = lexer.NextToken();

        while (_current.Kind != TokenKind.EndOfFile)
        {
            if (_current.Kind != TokenKind.Fn)
            {
                throw new Exception($"Unexpected token {_current.Kind} at position {_current.Position}.");
            }

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

    private List<Parameter> ParseParameters()
    {
        var parameters = new List<Parameter>();

        if (_current.Kind == TokenKind.RightParen)
        {
            return parameters;
        }

        parameters.Add(ParseParameter());

        while (_current.Kind == TokenKind.Comma)
        {
            Advance();
            parameters.Add(ParseParameter());
        }

        return parameters;
    }

    private Parameter ParseParameter()
    {
        var name = Expect(TokenKind.Identifier);
        Expect(TokenKind.Colon);
        var type = Expect(TokenKind.Identifier);

        return new Parameter(name.Lexeme, type.Lexeme);
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
            TokenKind.Let => ParseLetStatement(),
            _ => throw new Exception(
                $"Unexpected token {_current.Kind} at position {_current.Position}.")
        };
    }

    private LetStatement ParseLetStatement()
    {
        Expect(TokenKind.Let);

        var name = Expect(TokenKind.Identifier);

        Expect(TokenKind.Colon);

        var type = Expect(TokenKind.Identifier);

        Expect(TokenKind.Equals);

        var initializer = ParseExpression();

        Expect(TokenKind.Semicolon);

        return new LetStatement(
            name.Lexeme,
            type.Lexeme,
            initializer);
    }

    private ReturnStatement ParseReturnStatement()
    {
        Expect(TokenKind.Return);

        Expression? value = null;

        if (_current.Kind != TokenKind.Semicolon)
        {
            value = ParseExpression();
        }

        Expect(TokenKind.Semicolon);

        return new ReturnStatement(value);
    }

    private Expression ParseExpression()
    {
        var left = ParsePrimaryExpression();

        while (_current.Kind == TokenKind.Plus)
        {
            var operatorKind = _current.Kind;
            Advance();

            var right = ParsePrimaryExpression();

            left = new BinaryExpression(
                left,
                operatorKind,
                right);
        }

        return left;
    }

    private Expression ParsePrimaryExpression()
    {
        return _current.Kind switch
        {
            TokenKind.IntegerLiteral => ParseIntegerLiteral(),
            TokenKind.Identifier => ParseIdentifierExpression(),

            _ => throw new Exception(
                $"Unexpected token {_current.Kind} at position {_current.Position}.")
        };
    }

    private IntegerLiteralExpression ParseIntegerLiteral()
    {
        var token = Expect(TokenKind.IntegerLiteral);

        return new IntegerLiteralExpression(
            long.Parse(token.Lexeme));
    }

    private IdentifierExpression ParseIdentifierExpression()
    {
        var token = Expect(TokenKind.Identifier);

        return new IdentifierExpression(token.Lexeme);
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