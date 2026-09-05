namespace Sigil.Compiler.Syntax;

public class Parser(Lexer lexer)
{
    public Module Parse()
    {
        var declarations = new List<Declaration>();

        var token = lexer.NextToken();

        if (token.Kind == TokenKind.Fn)
        {
            declarations.Add(ParseFunction());
        }

        return new Module(declarations);
    }

    private FunctionDeclaration ParseFunction()
    {
        var name = Expect(TokenKind.Identifier);

        Expect(TokenKind.LeftParen);

        var parameters = ParseParameters();

        Expect(TokenKind.RightParen);
        Expect(TokenKind.LeftBrace);

        var body = ParseBlock();

        Expect(TokenKind.RightBrace);

        return new FunctionDeclaration(
            name.Lexeme,
            parameters,
            body);
    }

    private List<string> ParseParameters()
    {
        return [];
    }

    private Block ParseBlock()
    {
        return new Block([]);
    }

    private Token Expect(TokenKind kind)
    {
        var token = lexer.NextToken();

        if (token.Kind != kind)
        {
            throw new Exception(
                $"Expected {kind}, but found {token.Kind} at position {token.Position}.");
        }

        return token;
    }
}