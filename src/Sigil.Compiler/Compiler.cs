using Sigil.Compiler.CodeGen;
using Sigil.Compiler.Semantics;
using Sigil.Compiler.Syntax;

namespace Sigil.Compiler;

public sealed class Compiler
{
    public void Compile(
        string source,
        string outputPath)
    {
        var module = Parse(source);

        var boundModule = new NameResolver()
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