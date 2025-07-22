using CommandLine;
using Sigil.ModuleImporting;

namespace Sigil;


public class Stmt { }

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

    public static int Main(string[] args)
    {
        return Parser
        .Default
        .ParseArguments<RunOptions, BuildOptions>(args)
        .MapResult(
            (RunOptions opts) => RunFile(opts),
            (BuildOptions opts) => BuildNative(opts),
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
        // Lex
        // Parse
        // TypeCheck
        // JIT or Native Compile
        Console.WriteLine(sourceCode);
    }


    private static int BuildNative(BuildOptions opts)
    {
        if (opts.SourceFile is null)
        {
            Console.WriteLine("Source file is required.");
            return 1;
        }

        var sourceCode = FileLoader.LoadSourceCode(opts.SourceFile);
        Console.WriteLine(sourceCode);
        return 0;
    }
}
