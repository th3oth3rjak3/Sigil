using Sigil.Common;
using Sigil.ErrorHandling;

namespace Sigil.Lexing;

/// <summary>
/// Lexer scans the input source code and generates tokens.
/// </summary>
/// <param name="SourceCode">The input source code to be tokenized.</param>
public class Lexer(string SourceCode, ErrorHandler ErrorHandler)
{
    /// <summary>
    /// The current line number in the source code (1 based).
    /// </summary>
    private int _line = 1;

    /// <summary>
    /// The current column number in the source code (1 based).
    /// </summary>
    private int _column = 1;

    /// <summary>
    /// The number of characters from the start of the source code (0 based).
    /// </summary>
    private int _offset = 0;

    /// <summary>
    /// The number of characters from the start of the current line (0 based).
    /// </summary>
    private int _lineOffset = 0;

    /// <summary>
    /// The collection of tokens produced as the result of iterating over the source code.
    /// </summary>
    private List<Token> _tokens = [];

    /// <summary>
    /// The current position of the lexer in the source code.
    /// </summary>
    private Position _currentPosition => new(_line, _column, _offset, _lineOffset);

    /// <summary>
    /// Advance the position of the lexer in the source code.
    /// </summary>
    private void Advance()
    {
        throw new NotImplementedException("Advance not yet implemented");
    }

    /// <summary>
    /// Peek looks at the character in the source code at the current position without consuming it.
    /// </summary>
    /// <returns>The character at the current position when not at the end. If at the end of the
    /// source code, None is returned instead.</returns>
    private Option<char> Peek()
    {
        throw new NotImplementedException("Peek not yet implemented");
    }

    /// <summary>
    /// PeekNext looks at the character 1 position to the right of the current position in the source code
    /// without consuming it.
    /// </summary>
    /// <returns>The character at the current position + 1 when not at the end. If at the end of the
    /// source code, None is returned instead.</returns>
    private Option<char> PeekNext()
    {
        throw new NotImplementedException("PeekNext not yet implemented");
    }

    /// <summary>
    /// Make a token that has a single character like '(' or '.'
    /// </summary>
    /// <param name="tokenType">The type of the token.</param>
    /// <param name="startPosition">The starting position of the token in the source code.</param>
    /// <returns>A token.</returns>
    private Token MakeOneCharacterToken(TokenType tokenType, Position startPosition)
    {
        throw new NotImplementedException("MakeOneCharacterToken not yet implmented");
    }

    /// <summary>
    /// Make a token that has two characters like "<=" or "+="
    /// </summary>
    /// <param name="tokenType">The type of the token.</param>
    /// <param name="expected">The next expected character in the source code.</param>
    /// <param name="startPosition">The starting position of the token in the source code.</param>
    /// <returns>A Token when the expected character matches the next character in the source, otherwise None.</returns>
    private Option<Token> MakeTwoCharacterToken(TokenType tokenType, char expected, Position startPosition)
    {
        throw new NotImplementedException("MakeTwoCharacterToken not yet implmented");
    }

    /// <summary>
    /// Produce a list of tokens from the source code.
    /// </summary>
    /// <returns>The list of tokens produced by lexing.</returns>
    public List<Token> Tokenize()
    {
        throw new NotImplementedException("Tokenize not yet implemented");
    }
}
