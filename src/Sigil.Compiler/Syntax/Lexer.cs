namespace Sigil.Compiler.Syntax;

public sealed class Lexer
{
    private readonly string _source;
    private int _position;

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

        var start = _position;
        var current = _source[_position];

        if (char.IsLetter(current) || current == '_')
        {
            return ReadIdentifier(start);
        }

        throw new Exception($"Unexpected character '{current}' at position {_position}.");
    }

    private void SkipWhitespace()
    {
        while (_position < _source.Length &&
               char.IsWhiteSpace(_source[_position]))
        {
            _position++;
        }
    }

    private Token ReadIdentifier(int start)
    {
        while (_position < _source.Length &&
               (char.IsLetterOrDigit(_source[_position]) ||
                _source[_position] == '_'))
        {
            _position++;
        }

        var length = _position - start;
        var lexeme = _source.Substring(start, length);

        var kind = lexeme switch
        {
            "let" => TokenKind.Let,
            _ => TokenKind.Identifier
        };

        return new Token(kind, lexeme, start, length);
    }
}