using Sigil.Compiler.CodeGen;
using Sigil.Compiler.Semantics;
using Sigil.Compiler.Semantics.Primitives;
using Sigil.Compiler.Syntax;
using Sigil.Compiler.Syntax.Primitives;

namespace Sigil.Compiler;

public sealed class Compiler
{
    private readonly BuiltinRegistry _builtins = new();

    public void Compile(
        string source,
        string outputPath)
    {
        var module = Parse(source);

        var boundModule = new NameResolver(_builtins)
            .Resolve(module);

        var typedModule = new TypeChecker()
            .Check(boundModule);

        var llvmIr = new LlvmCodeGenerator()
            .Generate(typedModule);

        new NativeCompiler()
            .Compile(llvmIr, outputPath);
    }

    private static Module Parse(string source)
    {
        var lexer = new Lexer(source);
        var parser = new Parser(lexer);

        return parser.Parse();
    }
}
