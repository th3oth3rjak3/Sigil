namespace Sigil.Compiler.Syntax.Primitives;

public readonly record struct Token(
    TokenKind Kind,
    string Lexeme,
    int Position,
    int Length
);
