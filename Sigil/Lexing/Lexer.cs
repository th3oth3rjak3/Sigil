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
        Peek()
        .EffectSome(currentChar =>
        {
            _offset++;

            if (currentChar == '\n')
            {
                _line++;
                _lineOffset = _offset;
                _column = 1;
            }
            else
            {
                _column++;
            }

        });
    }

    /// <summary>
    /// Peek looks at the character in the source code at the current position without consuming it.
    /// </summary>
    /// <returns>The character at the current position when not at the end. If at the end of the
    /// source code, None is returned instead.</returns>
    private Option<char> Peek() => PeekAt(_offset);

    /// <summary>
    /// PeekNext looks at the character 1 position to the right of the current position in the source code
    /// without consuming it.
    /// </summary>
    /// <returns>The character at the current position + 1 when not at the end. If at the end of the
    /// source code, None is returned instead.</returns>
    private Option<char> PeekNext() => PeekAt(_offset + 1);

    /// <summary>
    /// PeekAt looks at the character at the provided offset position in the source code without consuming it.
    /// </summary>
    /// <param name="offset">The offset from the start of the source code, 0 based.</param>
    /// <returns>A character when the offset fits inside the source code length, otherwise None.</returns>
    private Option<char> PeekAt(int offset)
    {
        if (IsAtEnd(offset))
        {
            return None<char>();
        }

        return Some(SourceCode[offset]);
    }

    /// <summary>
    /// IsAtEnd checks to see if the lexer has reached the end of the source code at the given offset.
    /// </summary>
    /// <param name="offset">The index position of the character to check.</param>
    /// <returns>True if all characters have been processed, otherwise false.</returns>
    private bool IsAtEnd(int offset)
    {
        if (SourceCode.Length == 0)
        {
            return true;
        }

        return offset >= SourceCode.Length;
    }

    /// <summary>
    /// Make a token that has a single character like ( or .
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
        throw new NotImplementedException("MakeTwoCharacterToken not yet implemented");
    }

    /// <summary>
    /// SkipWhitespace advances over and discards any whitespace characters since the language
    /// does not consider them to be significant.
    /// </summary>
    private void SkipWhitespace()
    {
        throw new NotImplementedException("SkipWhitespace not implemented.");
    }

    /// <summary>
    /// SkipBasicComments advances over and discards any comments that are just used for
    /// simple remarks. This includes any comments that start with the "//" pattern.
    /// This function does not skip over docstring comments which have 3 slashes like "///".
    /// </summary>
    private void SkipBasicComments()
    {
        throw new NotImplementedException("SkipBasicComments not implemented.");
    }

    /// <summary>
    /// ReadDocstringComment lexes docstring comments and creates tokens so these
    /// comments can be used later in language tooling.
    /// </summary>
    /// <returns>A token representing a docstring comment.</returns>
    private Token ReadDocstringComment()
    {
        throw new NotImplementedException("ReadDocstringComment not implemented");
    }

    /// <summary>
    /// ReadNumber lexes a numeric token, either a float or an int since those are the only
    /// two types of numbers in the language.
    /// </summary>
    /// <returns>A numeric token.</returns>
    private Token ReadNumber()
    {
        throw new NotImplementedException("ReadNumber not implemented");
    }

    /// <summary>
    /// ReadChar lexes a character literal token.
    /// </summary>
    /// <returns>A character token.</returns>
    private Token ReadChar()
    {
        throw new NotImplementedException("ReadChar not implemented");
    }

    /// <summary>
    /// ReadString lexes a string literal token.
    /// </summary>
    /// <returns>A string literal token.</returns>
    private Token ReadString()
    {
        throw new NotImplementedException("ReadString not implemented.");
    }

    /// <summary>
    /// ReadTokens produces zero or more tokens by advancing through the source code.
    /// This function will generally produce a single token. However, there may be
    /// times during complex lexing, that a bunch of tokens are produced as the result
    /// of lexing something like an interpolated string, for instance. Zero tokens does not
    /// indicate an error, just that no tokens were creatable from the current point in the lexing process.
    /// When the last token in the output is an EOF token, processing is complete.
    /// </summary>
    /// <returns>A list containing zero or more tokens, usually just a single token.</returns>
    private List<Token> ReadTokens()
    {
        throw new NotImplementedException("ReadTokens not implemented");
    }

    /// <summary>
    /// IsLetter checks to see if a character is a valid letter in the language.
    /// </summary>
    /// <param name="ch">The character to check.</param>
    /// <returns>True when it is a letter, otherwise false.</returns>
    private static bool IsLetter(char ch)
    {
        throw new NotImplementedException("IsLetter not implemented");
    }

    /// <summary>
    /// IsNumber checks to see if a character is a valid number in the language.
    /// </summary>
    /// <param name="ch">The character to check.</param>
    /// <returns>True when it's a number, otherwise false.</returns>
    private static bool IsNumber(char ch)
    {
        throw new NotImplementedException("IsNumber not implemented");
    }

    /// <summary>
    /// IsUnderscore checks to see if the input character is an underscore '_'
    /// </summary>
    /// <param name="ch">The character to check.</param>
    /// <returns>True when it's an underscore, otherwise false.</returns>
    private static bool IsUnderscore(char ch) => ch == '_';

    /// <summary>
    /// Produce a list of tokens from the source code.
    /// </summary>
    /// <returns>The list of tokens produced by lexing.</returns>
    public List<Token> Tokenize()
    {
        while (true)
        {
            var tokens = ReadTokens();
            _tokens.AddRange(tokens);
            if (tokens.Count > 0 && tokens.Last().TokenType == TokenType.Eof)
            {
                break;
            }
        }

        return _tokens;
    }
}
