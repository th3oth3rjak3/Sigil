using Sigil.Compiler.CodeGen;
using Sigil.Compiler.Semantics;
using Sigil.Compiler.Syntax;

namespace Sigil.Compiler.Tests.CodeGen;

public sealed class LlvmCodeGeneratorTests
{
    [Fact]
    public void GeneratesMainFunctionReturningIntegerLiteral()
    {
        var module = Parse("""
            fn main() -> Integer {
                return 42;
            }
            """);

        var bound = new NameResolver().Resolve(module);
        var typed = new TypeChecker().Check(bound);

        var ir = new LlvmCodeGenerator().Generate(typed);

        Assert.Contains("define i64 @main()", ir);
        Assert.Contains("ret i64 42", ir);
    }

    private static Module Parse(string source)
    {
        var lexer = new Lexer(source);
        var parser = new Parser(lexer);

        return parser.Parse();
    }

    [Fact]
    public void GeneratesAdditionExpression()
    {
        var module = Parse("""
        fn main() -> Integer {
            let x: Integer = 20;
            let y: Integer = 22;
            return x + y;
        }
        """);

        var bound = new NameResolver().Resolve(module);
        var typed = new TypeChecker().Check(bound);

        var ir = new LlvmCodeGenerator().Generate(typed);

        Assert.Contains("add i64", ir);
    }

    [Fact]
    public void GeneratesSubtractionExpression()
    {
        var module = Parse("""
        fn main() -> Integer {
            let x: Integer = 42;
            let y: Integer = 20;
            return x - y;
        }
        """);

        var bound = new NameResolver().Resolve(module);
        var typed = new TypeChecker().Check(bound);

        var ir = new LlvmCodeGenerator().Generate(typed);

        Assert.Contains("sub i64", ir);
    }

    [Fact]
    public void GeneratesMultiplicationExpression()
    {
        var module = Parse("""
        fn main() -> Integer {
            let x: Integer = 20;
            let y: Integer = 22;
            return x * y;
        }
        """);

        var bound = new NameResolver().Resolve(module);
        var typed = new TypeChecker().Check(bound);

        var ir = new LlvmCodeGenerator().Generate(typed);

        Assert.Contains("mul i64", ir);
    }
}
