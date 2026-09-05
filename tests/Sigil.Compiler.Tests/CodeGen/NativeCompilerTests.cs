using System.Diagnostics;
using Sigil.Compiler.CodeGen;

namespace Sigil.Compiler.Tests.CodeGen;

public sealed class NativeCompilerTests
{
    [Fact]
    public void CompilesLlvmIrToNativeExecutable()
    {
        const string llvmIr = """
            define i64 @main() {
            entry:
              ret i64 42
            }
            """;

        const string outputPath = "/tmp/sigil-test";

        try
        {
            var compiler = new NativeCompiler();

            compiler.Compile(llvmIr, outputPath);

            Assert.True(File.Exists(outputPath));

            var startInfo = new ProcessStartInfo
            {
                FileName = outputPath,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
            };

            using var process = Process.Start(startInfo)
                ?? throw new Exception(
                    "Failed to start compiled executable.");

            process.WaitForExit();

            Assert.Equal(42, process.ExitCode);
        }
        finally
        {
            File.Delete(outputPath);
        }
    }
}