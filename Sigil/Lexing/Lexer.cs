using Sigil.Common;

namespace Sigil.Lexing;

public class Lexer(string SourceCode)
{
    private int _line = 1;
    private int _column = 1;
    private int _offset = 0;

    public Position CurrentPosition => new(_line, _column, _offset);
}
