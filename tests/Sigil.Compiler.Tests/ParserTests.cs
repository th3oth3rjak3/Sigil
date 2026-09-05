using Sigil.Compiler.Syntax;

namespace Sigil.Compiler.Tests;

public class ParserTests
{
    [Fact]
    public void ParsesFunctionDeclaration()
    {
        var parser = new Parser(new Lexer("fn main() {}"));

        var module = parser.Parse();

        Assert.Single(module.Declarations);

        var function = Assert.IsType<FunctionDeclaration>(
            module.Declarations[0]);

        Assert.Equal("main", function.Name);
        Assert.Empty(function.Parameters);
        Assert.Empty(function.Body.Statements);
    }
}