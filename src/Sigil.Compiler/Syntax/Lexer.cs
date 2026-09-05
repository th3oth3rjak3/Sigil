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
            ';' => MakeToken(TokenKind.Semicolon, 1),
            var ch when char.IsLetter(ch) || ch == '_' => ReadIdentifier(),
            _ => throw new Exception($"Unexpected character '{current}' at position {_position}.")

        };

        return token;
    }

    private Token MakeToken(TokenKind kind, int length)
    {
        var lexeme = _source.Substring(_tokenStart, length);
        _position += length;
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
            _ => TokenKind.Identifier
        };

        return new Token(kind, lexeme, _tokenStart, length);
    }
}