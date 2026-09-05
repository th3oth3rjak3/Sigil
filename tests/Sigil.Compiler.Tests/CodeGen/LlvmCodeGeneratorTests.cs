using Sigil.Compiler.CodeGen;
using Sigil.Compiler.Semantics;
using Sigil.Compiler.Syntax;
using Sigil.Compiler.Syntax.Declarations;
using Sigil.Compiler.Syntax.Primitives;

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

        var bound = new NameResolver(new BuiltinRegistry()).Resolve(module);
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

        var bound = new NameResolver(new BuiltinRegistry()).Resolve(module);
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

        var bound = new NameResolver(new BuiltinRegistry()).Resolve(module);
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

        var bound = new NameResolver(new BuiltinRegistry()).Resolve(module);
        var typed = new TypeChecker().Check(bound);

        var ir = new LlvmCodeGenerator().Generate(typed);

        Assert.Contains("mul i64", ir);
    }

    [Fact]
    public void GeneratesIntegerDivisionExpression()
    {
        var module = Parse("""
        fn main() -> Integer {
            let x: Integer = 42;
            let y: Integer = 2;
            return x / y;
        }
        """);

        var bound = new NameResolver(new BuiltinRegistry()).Resolve(module);
        var typed = new TypeChecker().Check(bound);

        var ir = new LlvmCodeGenerator().Generate(typed);

        Assert.Contains("sdiv i64", ir);
    }

    [Fact]
    public void GeneratesFloatDivisionExpression()
    {
        var module = Parse("""
        fn main() -> Float {
            let x: Float = 42.0;
            let y: Float = 2.0;
            return x / y;
        }
        """);

        var bound = new NameResolver(new BuiltinRegistry()).Resolve(module);
        var typed = new TypeChecker().Check(bound);

        var ir = new LlvmCodeGenerator().Generate(typed);

        Assert.Contains("fdiv double", ir);
    }

    [Fact]
    public void Generate_DeclaresRuntimeFunctions()
    {
        var module = new TypedModule(
            [
                new TypedFunctionDeclaration(
                    new FunctionDeclaration(
                        "main",
                        [],
                        "Integer",
                        new Block([])),
                    new VoidType(),
                    new TypedBlock([]))
            ]);

        var generator = new LlvmCodeGenerator();

        var ir = generator.Generate(module);

        Assert.Contains(
            "declare void @sigil_println_integer(i64)",
            ir);

        Assert.Contains(
            "declare void @sigil_println_float(double)",
            ir);
    }

    [Fact]
    public void GeneratesFunctionCall()
    {
        var module = Parse("""
        fn foo() -> Integer {
            return 42;
        }

        fn main() -> Integer {
            return foo();
        }
        """);

        var bound = new NameResolver(new BuiltinRegistry()).Resolve(module);
        var typed = new TypeChecker().Check(bound);

        var ir = new LlvmCodeGenerator().Generate(typed);

        Assert.Contains(
            "call i64 @foo()",
            ir);
    }
}
