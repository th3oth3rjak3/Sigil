using Sigil.Common;

namespace Sigil.Parsing.Statements;


public record FieldDeclaration(
    Span Span,
    string Name,
    string? TypeName = null // if your language has types yet
);
