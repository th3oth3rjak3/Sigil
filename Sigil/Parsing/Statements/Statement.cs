using Sigil.Common;

namespace Sigil.Parsing.Statements;

public abstract record Statement(Span Span) : AstNode(Span);
