using Sigil.Common;

namespace Sigil.Lexing;

public record Token(TokenType TokenType, Span Span);
