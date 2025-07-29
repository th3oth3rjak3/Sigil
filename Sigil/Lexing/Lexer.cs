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
    /// The previous position, just before the current position.
    /// </summary>
    private Position _lastPosition = new(1, 1, 0, 0);

    /// <summary>
    /// Advance the position of the lexer in the source code.
    /// </summary>
    private void Advance()
    {
        Peek()
        .EffectSome(currentChar =>
        {
            _lastPosition = _currentPosition;

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
    /// IsStartOfDocstringComment checks to see if the lexer is currently at the start of a docstring comment.
    /// </summary>
    /// <returns>Returns true when at the start of a docstring comment, otherwise false.</returns>
    private bool IsStartOfDocstringComment()
    {
        var currentIsSlash = Peek().Match(some => some == '/', () => false);
        var nextIsSlash = PeekNext().Match(some => some == '/', () => false);
        var thirdIsSlash = PeekAt(_offset + 2).Match(some => some == '/', () => false);

        return currentIsSlash && nextIsSlash && thirdIsSlash;
    }

    /// <summary>
    /// Make a token that has a single character like ( or .
    /// </summary>
    /// <param name="tokenType">The type of the token.</param>
    /// <param name="startPosition">The starting position of the token in the source code.</param>
    /// <returns>A token.</returns>
    private Token MakeOneCharacterToken(TokenType tokenType, Position startPosition)
    {
        Advance();
        return new Token(tokenType, new Span(startPosition, _lastPosition));
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
        var peekResult = PeekNext();
        if (peekResult.IsNone || peekResult.Unwrap() != expected) return None<Token>();

        Advance();
        Advance();
        return new Token(tokenType, new Span(startPosition, _lastPosition));
    }
    /// <summary>
    /// SkipWhitespace advances over and discards any whitespace characters since the language
    /// does not consider them to be significant.
    /// </summary>
    private void SkipWhitespace()
    {
        while (true)
        {
            var maybeChar = Peek();
            if (maybeChar.IsNone) return; // must have reached the end of the source code.

            var ch = maybeChar.Unwrap();
            if (char.IsWhiteSpace(ch))
            {
                Advance();
            }
            else
            {
                return;
            }
        }
    }

    /// <summary>
    /// SkipBasicComments advances over and discards any comments that are just used for
    /// simple remarks. This includes any comments that start with the "//" pattern.
    /// This function does not skip over docstring comments which have 3 slashes like "///".
    /// </summary>
    private void SkipBasicComments()
    {
        while (true)
        {
            var maybeChar = Peek();
            if (maybeChar.IsNone) return;

            var ch = maybeChar.Unwrap();
            if (ch == '\n')
            {
                Advance(); // skip the newline to go the start of the next line.
                return;
            }

            Advance();
        }
    }

    private void SkipWhitespaceAndComments()
    {
        while (true)
        {
            SkipWhitespace();
            var currentIsSlash = Peek().Match(some => some == '/', () => false);
            var nextIsSlash = PeekNext().Match(some => some == '/', () => false);
            var thirdIsSlash = PeekAt(_offset + 2).Match(some => some == '/', () => false);

            // Basic Comment has // instead of ///
            if (currentIsSlash && nextIsSlash && !thirdIsSlash)
            {
                SkipBasicComments();
            }
            else
            {
                return;
            }
        }
    }

    /// <summary>
    /// ReadDocstringComment lexes docstring comments and creates tokens so these
    /// comments can be used later in language tooling.
    /// </summary>
    /// <returns>A token representing a docstring comment.</returns>
    private Token ReadDocstringComment(Position startPosition)
    {
        while (IsStartOfDocstringComment())
        {
            while (Peek().IsSome && Peek().Unwrap() != '\n')
            {
                Advance();
            }

            SkipWhitespace();
        }

        return new Token(TokenType.DocStringComment, new Span(startPosition, _lastPosition));
    }

    /// <summary>
    /// ReadNumber lexes a numeric token, either a float or an int since those are the only
    /// two types of numbers in the language.
    /// </summary>
    /// <returns>A numeric token.</returns>
    private Token ReadNumber(Position startPosition)
    {
        while (Peek().IsSome && IsNumber(Peek().Unwrap()))
        {
            Advance();
        }

        if (Peek().IsSome && Peek().Unwrap() == '.' && PeekNext().IsSome && IsNumber(PeekNext().Unwrap()))
        {
            Advance();

            while (Peek().IsSome && IsNumber(Peek().Unwrap()))
            {
                Advance();
            }

            return new Token(TokenType.FloatLiteral, new Span(startPosition, _lastPosition));
        }

        return new Token(TokenType.IntegerLiteral, new Span(startPosition, _lastPosition));
    }

    /// <summary>
    /// ReadChar lexes a character literal token.
    /// </summary>
    /// <returns>A character token.</returns>
    private Token ReadChar(Position startPosition)
    {
        throw new NotImplementedException("ReadChar not implemented");
    }

    /// <summary>
    /// ReadString lexes a string literal token.
    /// </summary>
    /// <returns>A string literal token.</returns>
    private Token ReadString(Position startPosition)
    {
        Advance(); // consume "
        while (!IsAtEnd(_offset) && Peek() != '"')
        {
            Advance();
        }

        if (IsAtEnd(_offset) && Peek() != '"')
        {
            var span = new Span(startPosition, _lastPosition);
            ErrorHandler.Report("Unterminated String", span);
            return new Token(TokenType.Invalid, span);
        }

        Advance(); // consume closing "

        return new Token(TokenType.StringLiteral, new Span(startPosition, _lastPosition));
    }

    /// <summary>
    /// ReadIdentifier creates a token that is either a keyword or a user generated let binding.
    /// </summary>
    /// <param name="startPosition">The start position of the current span.</param>
    /// <returns>An identifier token.</returns>
    private Token ReadIdentifier(Position startPosition)
    {
        while (true)
        {
            var maybeChar = Peek();
            if (maybeChar.IsNone)
            {
                break;
            }
            else
            {
                var ch = maybeChar.Unwrap();

                if (IsLetter(ch) || IsNumber(ch) || IsUnderscore(ch))
                {
                    Advance();
                }
                else
                {
                    break;
                }
            }
        }

        var span = new Span(startPosition, _lastPosition);
        var lexeme = span.Slice(SourceCode);

        // Need to check if it's a keyword or a user generate identifier.
        var maybeKeyword = Keywords.FromString(lexeme);
        if (maybeKeyword.IsSome)
        {
            var keyword = maybeKeyword.Unwrap();
            return new Token(keyword, span);
        }

        return new Token(TokenType.Identifier, span);
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
        SkipWhitespaceAndComments();

        var startPosition = _currentPosition;

        var maybeChar = Peek();

        if (maybeChar.IsNone)
        {
            var token = new Token(TokenType.Eof, new Span(startPosition, startPosition));
            return [token];
        }

        var currentChar = maybeChar.Unwrap();

        if (IsNumber(currentChar))
        {
            return [ReadNumber(startPosition)];
        }

        if (IsLetter(currentChar))
        {
            return [ReadIdentifier(startPosition)];
        }

        List<Token> tokens = [];

        switch (currentChar)
        {
            // Literals
            case '"':
                var token = ReadString(startPosition);
                tokens.Add(token);
                break;
            case '\'':
                token = ReadChar(startPosition);
                tokens.Add(token);
                break;

            // Delimiters
            case '(':
                addOneCharToken(TokenType.LeftParen); break;
            case ')':
                addOneCharToken(TokenType.RightParen); break;
            case '{':
                addOneCharToken(TokenType.LeftBrace); break;
            case '}':
                addOneCharToken(TokenType.RightBrace); break;
            case '[':
                addOneCharToken(TokenType.LeftBracket); break;
            case ']':
                addOneCharToken(TokenType.RightBracket); break;
            case ',':
                addOneCharToken(TokenType.Comma); break;
            case ';':
                addOneCharToken(TokenType.Semicolon); break;
            case ':':
                addOneCharToken(TokenType.Colon); break;
            case '.':
                addOneCharToken(TokenType.Dot); break;

            // Operators
            case '+':
                var maybeToken = MakeTwoCharacterToken(TokenType.PlusEqual, '=', startPosition);
                if (maybeToken.IsSome)
                {
                    tokens.Add(maybeToken.Unwrap());
                    break;
                }

                addOneCharToken(TokenType.Plus);
                break;
            case '-':
                maybeToken = MakeTwoCharacterToken(TokenType.MinusEqual, '=', startPosition);
                if (maybeToken.IsSome)
                {
                    tokens.Add(maybeToken.Unwrap());
                    break;
                }

                maybeToken = MakeTwoCharacterToken(TokenType.Arrow, '>', startPosition);
                if (maybeToken.IsSome)
                {
                    tokens.Add(maybeToken.Unwrap());
                    break;
                }

                addOneCharToken(TokenType.Minus);
                break;

            case '*':
                maybeToken = MakeTwoCharacterToken(TokenType.StarEqual, '=', startPosition);
                if (maybeToken.IsSome)
                {
                    tokens.Add(maybeToken.Unwrap());
                    break;
                }

                addOneCharToken(TokenType.Star);
                break;

            case '/':
                if (IsStartOfDocstringComment())
                {
                    tokens.Add(ReadDocstringComment(startPosition));
                    break;
                }

                maybeToken = MakeTwoCharacterToken(TokenType.SlashEqual, '=', startPosition);
                if (maybeToken.IsSome)
                {
                    tokens.Add(maybeToken.Unwrap());
                    break;
                }

                addOneCharToken(TokenType.Slash);
                break;

            case '=':
                maybeToken = MakeTwoCharacterToken(TokenType.EqualEqual, '=', startPosition);
                if (maybeToken.IsSome)
                {
                    tokens.Add(maybeToken.Unwrap());
                    break;
                }

                maybeToken = MakeTwoCharacterToken(TokenType.FatArrow, '>', startPosition);
                if (maybeToken.IsSome)
                {
                    tokens.Add(maybeToken.Unwrap());
                    break;
                }

                addOneCharToken(TokenType.Equal);
                break;

            case '!':
                maybeToken = MakeTwoCharacterToken(TokenType.BangEqual, '=', startPosition);
                if (maybeToken.IsSome)
                {
                    tokens.Add(maybeToken.Unwrap());
                    break;
                }

                addOneCharToken(TokenType.Bang);
                break;

            case '<':
                maybeToken = MakeTwoCharacterToken(TokenType.LessEqual, '=', startPosition);
                if (maybeToken.IsSome)
                {
                    tokens.Add(maybeToken.Unwrap());
                    break;
                }

                addOneCharToken(TokenType.Less);
                break;

            case '>':
                maybeToken = MakeTwoCharacterToken(TokenType.GreaterEqual, '=', startPosition);
                if (maybeToken.IsSome)
                {
                    tokens.Add(maybeToken.Unwrap());
                    break;
                }

                addOneCharToken(TokenType.Greater);
                break;

            default:
                addOneCharToken(TokenType.Invalid);
                ErrorHandler.Report($"Unexpected Character '{currentChar}'", new Span(startPosition, _lastPosition));
                break;
        }

        return tokens;

        void addOneCharToken(TokenType tokenType) =>
            MakeOneCharacterToken(tokenType, startPosition)
            .Effect(tokens.Add);
    }

    /// <summary>
    /// IsLetter checks to see if a character is a valid letter in the language.
    /// </summary>
    /// <param name="ch">The character to check.</param>
    /// <returns>True when it is a letter, otherwise false.</returns>
    private static bool IsLetter(char ch) => char.IsAsciiLetter(ch);

    /// <summary>
    /// IsNumber checks to see if a character is a valid number in the language.
    /// </summary>
    /// <param name="ch">The character to check.</param>
    /// <returns>True when it's a number, otherwise false.</returns>
    private static bool IsNumber(char ch) => char.IsAsciiDigit(ch);

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
