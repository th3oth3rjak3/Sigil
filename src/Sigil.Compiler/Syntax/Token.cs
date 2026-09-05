namespace Sigil.Compiler.Syntax;

public readonly record struct Token(
    TokenKind Kind,
    string Lexeme,
    int Position,
    int Length
);