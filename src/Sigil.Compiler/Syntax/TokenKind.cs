namespace Sigil.Compiler.Syntax;

public enum TokenKind
{
    // Special
    EndOfFile,

    // Literals
    IntegerLiteral,
    FloatLiteral,
    StringLiteral,

    // Identifiers
    Identifier,

    // Keywords
    Let,
    Fn,
    Return,
    True,
    False,

    // Operators
    Plus,
    Minus,
    Star,
    Slash,
    Equals,
    EqualsEquals,

    // Punctuation
    LeftParen,
    RightParen,
    LeftBrace,
    RightBrace,
    Comma,
    Semicolon,
    Colon,
}