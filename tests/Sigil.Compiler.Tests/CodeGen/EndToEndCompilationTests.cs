using System.Diagnostics;

namespace Sigil.Compiler.Tests.CodeGen;

public sealed class EndToEndCompilationTests
{
    [Fact]
    public void CompilesAndRunsSigilSource()
    {
        const string source = """
            fn main() -> Integer {
                return 42;
            }
            """;

        const string outputPath = "/tmp/sigil-test";

        try
        {
            new Compiler().Compile(source, outputPath);

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

    [Fact]
    public void CompilesAndRunsAdditionExpression()
    {
        const string source = """
        fn main() -> Integer {
            let x: Integer = 20;
            let y: Integer = 22;
            return x + y;
        }
        """;

        const string outputPath = "/tmp/sigil-test-addition";

        try
        {
            new Compiler().Compile(source, outputPath);

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

    [Fact]
    public void CompilesAndRunsSubtractionExpression()
    {
        const string source = """
        fn main() -> Integer {
            let x: Integer = 42;
            let y: Integer = 20;
            return x - y;
        }
        """;

        const string outputPath = "/tmp/sigil-test-subtraction";

        try
        {
            new Compiler().Compile(source, outputPath);

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

            Assert.Equal(22, process.ExitCode);
        }
        finally
        {
            File.Delete(outputPath);
        }
    }
}