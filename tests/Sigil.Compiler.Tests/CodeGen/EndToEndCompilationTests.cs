using System.Diagnostics;

namespace Sigil.Compiler.Tests.CodeGen;

public sealed class EndToEndCompilationTests
{
    private sealed record ProcessResult(
        int ExitCode,
        string StandardOutput,
        string StandardError);

    private static ProcessResult CompileAndRun(string source)
    {
        var outputPath = Path.Combine(
            Path.GetTempPath(),
            OperatingSystem.IsWindows()
                ? $"sigil-test-{Guid.NewGuid():N}.exe"
                : $"sigil-test-{Guid.NewGuid():N}");

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

            return new ProcessResult(
                process.ExitCode,
                process.StandardOutput.ReadToEnd(),
                process.StandardError.ReadToEnd());
        }
        finally
        {
            File.Delete(outputPath);
        }
    }

    [Fact]
    public void CompilesAndRunsSigilSource()
    {
        const string source = """
            fn main() -> Integer {
                return 42;
            }
            """;

        var result = CompileAndRun(source);

        Assert.Equal(42, result.ExitCode);
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

        var result = CompileAndRun(source);

        Assert.Equal(42, result.ExitCode);
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

        var result = CompileAndRun(source);

        Assert.Equal(22, result.ExitCode);
    }

    [Fact]
    public void CompilesAndRunsMultiplicationExpression()
    {
        const string source = """
            fn main() -> Integer {
                return 5 * 2;
            }
            """;

        var result = CompileAndRun(source);

        Assert.Equal(10, result.ExitCode);
    }

    [Fact]
    public void CompilesAndRunsIntegerDivisionExpression()
    {
        const string source = """
        fn main() -> Integer {
            return 42 / 2;
        }
        """;

        var result = CompileAndRun(source);

        Assert.Equal(21, result.ExitCode);
    }

    [Fact]
    public void CompilesAndRunsFunctionCall()
    {
        const string source = """
        fn foo() -> Integer {
            return 42;
        }

        fn main() -> Integer {
            return foo();
        }
        """;

        var result = CompileAndRun(source);

        Assert.Equal(42, result.ExitCode);
    }

}
