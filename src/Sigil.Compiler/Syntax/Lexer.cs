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
        var current = _source[_position];

        var token = current switch
        {
            ';' => ReadSemicolon(),
            '(' => ReadLeftParen(),
            ')' => ReadRightParen(),
            '{' => ReadLeftBrace(),
            '}' => ReadRightBrace(),
            ',' => ReadComma(),
            ':' => ReadColon(),
            '-' => ReadDash(),
            '=' => ReadEquals(),
            '+' => ReadPlus(),
            '*' => ReadStar(),
            _ when char.IsLetter(current) || current == '_' => ReadIdentifier(),
            _ when char.IsDigit(current) => ReadNumber(),
            _ => throw new Exception($"Unexpected character '{current}' at position {_position}.")

        };

        return token;
    }

    private Token MakeToken(TokenKind kind)
    {
        var length = _position - _tokenStart;
        var lexeme = _source.Substring(_tokenStart, length);
        return new Token(kind, lexeme, _tokenStart, length);
    }

    private void SkipWhitespace()
    {
        while (_position < _source.Length &&
               char.IsWhiteSpace(_source[_position]))
        {
            _position++;
        }
    }

    private Token ReadIdentifier()
    {
        while (_position < _source.Length &&
               (char.IsLetterOrDigit(_source[_position]) ||
                _source[_position] == '_'))
        {
            _position++;
        }

        var length = _position - _tokenStart;
        var lexeme = _source.Substring(_tokenStart, length);

        var kind = lexeme switch
        {
            "let" => TokenKind.Let,
            "fn" => TokenKind.Fn,
            "return" => TokenKind.Return,
            _ => TokenKind.Identifier
        };

        return new Token(kind, lexeme, _tokenStart, length);
    }

    private Token ReadNumber()
    {
        while (_position < _source.Length && char.IsDigit(_source[_position]))
        {
            _position++;
        }

        return MakeToken(TokenKind.IntegerLiteral);
    }

    private Token ReadSemicolon()
    {
        _position++;
        return MakeToken(TokenKind.Semicolon);
    }

    private Token ReadLeftParen()
    {
        _position++;
        return MakeToken(TokenKind.LeftParen);
    }

    private Token ReadRightParen()
    {
        _position++;
        return MakeToken(TokenKind.RightParen);
    }

    private Token ReadLeftBrace()
    {
        _position++;
        return MakeToken(TokenKind.LeftBrace);
    }

    private Token ReadRightBrace()
    {
        _position++;
        return MakeToken(TokenKind.RightBrace);
    }

    private Token ReadComma()
    {
        _position++;
        return MakeToken(TokenKind.Comma);
    }

    private Token ReadColon()
    {
        _position++;
        return MakeToken(TokenKind.Colon);
    }

    private Token ReadDash()
    {
        // Look at the next character without consuming it.
        if (_position + 1 < _source.Length &&
            _source[_position + 1] == '>')
        {
            _position += 2;
            return MakeToken(TokenKind.Arrow);
        }

        _position++;
        return MakeToken(TokenKind.Minus);
    }

    private Token ReadEquals()
    {
        _position++;

        if (_position < _source.Length && _source[_position] == '=')
        {
            _position++;

            return new Token(
                TokenKind.EqualsEquals,
                "==",
                _tokenStart,
                2);
        }

        return new Token(
            TokenKind.Equals,
            "=",
            _tokenStart,
            1);
    }

    private Token ReadPlus()
    {
        _position++;
        return new Token(TokenKind.Plus, "+", _tokenStart, 1);
    }

    private Token ReadStar()
    {
        _position++;
        return new Token(TokenKind.Star, "*", _tokenStart, 1);
    }
}
