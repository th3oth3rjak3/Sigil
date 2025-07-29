namespace Sigil.Lexing;

public enum TokenType
{
    // Literals
    IntegerLiteral,
    FloatLiteral,
    StringLiteral,
    InterpolatedStringStart,
    InterpolatedStringMiddle,
    InterpolatedStringEnd,
    CharacterLiteral,

    // Identifiers
    Identifier,

    // Keywords
    Let,
    Fun,
    Class,
    New,
    This,
    If,
    Else,
    While,
    For,
    Return,
    True,
    False,
    Break,
    Continue,
    Or,
    And,

    // Operators
    Plus,
    PlusEqual,
    Minus,
    MinusEqual,
    Star,
    StarEqual,
    Slash,
    SlashEqual,
    Equal,
    EqualEqual,
    Bang,
    BangEqual,
    Less,
    LessEqual,
    Greater,
    GreaterEqual,
    Arrow,
    FatArrow,

    // Delimiters
    LeftParen,
    RightParen,
    LeftBrace,
    RightBrace,
    LeftBracket,
    RightBracket,
    Comma,
    Semicolon,
    Colon,
    Dot,

    // Special
    Newline,
    Whitespace,
    Comment,
    DocStringComment,
    Eof,

    // Errors
    Invalid,
}
