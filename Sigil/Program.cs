using System.Diagnostics.CodeAnalysis;

using CommandLine;
using Sigil.Debugging;
using Sigil.ErrorHandling;
using Sigil.Interpretation;
using Sigil.Lexing;
using Sigil.ModuleImporting;

namespace Sigil;

[ExcludeFromCodeCoverage]
class Program
{
    [Verb("run", HelpText = "Run in debug mode.")]
    public class RunOptions
    {
        [Option('f', "file", Required = true, HelpText = "The main source file")]
        public string? SourceFile { get; set; }
    }

    [Verb("build", HelpText = "Build native executable.")]
    public class BuildOptions
    {
        [Option('f', "file", Required = true, HelpText = "The main source file")]
        public string? SourceFile { get; set; }
    }

    [Verb("parse", HelpText = "Parse the source code and print the untyped AST")]
    public class ParseOptions
    {
        [Option('f', "file", Required = true, HelpText = "The main source file")]
        public string? SourceFile { get; set; }
    }

    [Verb("typecheck", HelpText = "Parse the source code and print the typed AST")]
    public class TypeCheckOptions
    {
        [Option('f', "file", Required = true, HelpText = "The main source file")]
        public string? SourceFile { get; set; }
    }

    public static int Main(string[] args)
    {
        return Parser
        .Default
        .ParseArguments<RunOptions, BuildOptions, ParseOptions, TypeCheckOptions>(args)
        .MapResult(
            (RunOptions opts) => RunFile(opts),
            (BuildOptions opts) => BuildNative(opts),
            (ParseOptions opts) => ParseFile(opts),
            (TypeCheckOptions opts) => TypeCheckFile(opts),
            errs => 1);
    }

    /// <summary>
    /// RunFile builds the source code using the LLVM JIT engine
    /// and runs it.
    /// </summary>
    /// <param name="opts">Options used to run the file.</param>
    private static int RunFile(RunOptions opts)
    {
        if (opts.SourceFile is null)
        {
            Console.WriteLine("Source file is required.");
            return 1;
        }

        var sourceCode = FileLoader.LoadSourceCode(opts.SourceFile);
        Run(sourceCode);

        return 0;
    }

    private static void Run(string sourceCode)
    {
        var backend = new Interpreter(sourceCode);
        var compiler = new Compiler(sourceCode, backend);
        var exitCode = compiler.Compile();
        System.Environment.Exit(exitCode);
    }

    private static int BuildNative(BuildOptions opts)
    {
        if (opts.SourceFile is null)
        {
            Console.WriteLine("Source file is required.");
            return 1;
        }

        // TODO: build the LLVM IR backend.
        var sourceCode = FileLoader.LoadSourceCode(opts.SourceFile);
        Console.WriteLine(sourceCode);
        return 0;
    }

    private static int ParseFile(ParseOptions opts)
    {
        if (opts.SourceFile is null)
        {
            Console.WriteLine("Source file is required.");
            return 1;
        }

        var sourceCode = FileLoader.LoadSourceCode(opts.SourceFile);

        var compiler = new Compiler(sourceCode, new AstPrintingVisitor());
        return compiler.Compile();
    }

    private static int TypeCheckFile(TypeCheckOptions opts)
    {
        if (opts.SourceFile is null)
        {
            Console.WriteLine("Source file is required.");
            return 1;
        }

        var sourceCode = FileLoader.LoadSourceCode(opts.SourceFile);

        var errorHandler = new ErrorHandler(sourceCode);
        var compiler = new Compiler(sourceCode, new TypeCheckedAstPrintingVisitor(errorHandler));
        var exitCode = compiler.Compile();
        if (errorHandler.HadError)
        {
            foreach (var error in errorHandler.Errors)
            {
                Console.WriteLine(error);
            }
        }

        return exitCode;
    }
}
