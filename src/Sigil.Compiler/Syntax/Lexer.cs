namespace Sigil.Compiler.Syntax;

public sealed class Lexer
{
    private readonly string _source;
    private int _position;
    private int _tokenStart;

    public Lexer(string source)
    {
        _source = source;
    }

    public Token NextToken()
    {
        SkipWhitespace();

        if (_position >= _source.Length)
        {
            return new Token(TokenKind.EndOfFile, string.Empty, _position, 0);
        }

        _tokenStart = _position;

        return Current switch
        {
            ';' => ReadSingleCharacterToken(TokenKind.Semicolon),
            '(' => ReadSingleCharacterToken(TokenKind.LeftParen),
            ')' => ReadSingleCharacterToken(TokenKind.RightParen),
            '{' => ReadSingleCharacterToken(TokenKind.LeftBrace),
            '}' => ReadSingleCharacterToken(TokenKind.RightBrace),
            ',' => ReadSingleCharacterToken(TokenKind.Comma),
            ':' => ReadSingleCharacterToken(TokenKind.Colon),
            '+' => ReadSingleCharacterToken(TokenKind.Plus),
            '*' => ReadSingleCharacterToken(TokenKind.Star),
            '/' => ReadSingleCharacterToken(TokenKind.Slash),
            '-' => ReadDash(),
            '=' => ReadEquals(),
            _ when char.IsLetter(Current) || Current == '_' => ReadIdentifier(),
            _ when char.IsDigit(Current) => ReadNumber(),
            _ => throw new Exception(
                $"Unexpected character '{Current}' at position {_position}.")
        };
    }

    private char Current =>
        _position < _source.Length
            ? _source[_position]
            : '\0';

    private char Peek(int offset = 1) =>
        _position + offset < _source.Length
            ? _source[_position + offset]
            : '\0';

    private void Advance()
    {
        _position++;
    }

    private Token MakeToken(TokenKind kind)
    {
        var length = _position - _tokenStart;

        return new Token(
            kind,
            _source.Substring(_tokenStart, length),
            _tokenStart,
            length);
    }

    private Token ReadSingleCharacterToken(TokenKind kind)
    {
        Advance();
        return MakeToken(kind);
    }

    private void SkipWhitespace()
    {
        while (char.IsWhiteSpace(Current))
        {
            Advance();
        }
    }

    private Token ReadIdentifier()
    {
        while (char.IsLetterOrDigit(Current) || Current == '_')
        {
            Advance();
        }

        var lexeme = _source.Substring(
            _tokenStart,
            _position - _tokenStart);

        var kind = lexeme switch
        {
            "let" => TokenKind.Let,
            "fn" => TokenKind.Fn,
            "return" => TokenKind.Return,
            _ => TokenKind.Identifier
        };

        return MakeToken(kind);
    }

    private Token ReadNumber()
    {
        while (char.IsDigit(Current))
        {
            Advance();
        }

        if (Current != '.')
        {
            return MakeToken(TokenKind.IntegerLiteral);
        }

        if (!char.IsDigit(Peek()))
        {
            throw new Exception("Invalid floating-point literal.");
        }

        Advance();

        while (char.IsDigit(Current))
        {
            Advance();
        }

        return MakeToken(TokenKind.FloatLiteral);
    }

    private Token ReadDash()
    {
        if (Peek() == '>')
        {
            Advance();
            Advance();
            return MakeToken(TokenKind.Arrow);
        }

        return ReadSingleCharacterToken(TokenKind.Minus);
    }

    private Token ReadEquals()
    {
        Advance();

        if (Current == '=')
        {
            Advance();
            return MakeToken(TokenKind.EqualsEquals);
        }

        return MakeToken(TokenKind.Equals);
    }
}
