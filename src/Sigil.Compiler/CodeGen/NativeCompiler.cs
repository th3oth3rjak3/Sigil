using System.Diagnostics;

namespace Sigil.Compiler.CodeGen;

public sealed class NativeCompiler
{
    private const string Clang = "clang";

    private static readonly string RuntimeLibrary =
        Path.Combine(
            AppContext.BaseDirectory,
            "sigil_runtime",
            "libsigil_runtime.a");

    public void Compile(
        string llvmIr,
        string outputPath)
    {
        var irPath = Path.Combine(
            Path.GetTempPath(),
            $"{Guid.NewGuid():N}.ll");

        try
        {
            File.WriteAllText(irPath, llvmIr);

            var startInfo = new ProcessStartInfo
            {
                FileName = Clang,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
            };

            startInfo.ArgumentList.Add(irPath);
            startInfo.ArgumentList.Add(RuntimeLibrary);
            startInfo.ArgumentList.Add("-o");
            startInfo.ArgumentList.Add(outputPath);

            using var process = Process.Start(startInfo)
                ?? throw new Exception(
                    "Failed to start clang.");

            var stdout = process.StandardOutput.ReadToEnd();
            var stderr = process.StandardError.ReadToEnd();

            process.WaitForExit();

            if (process.ExitCode != 0)
            {
                throw new Exception(
                    $"clang failed with exit code " +
                    $"{process.ExitCode}:\n{stderr}");
            }
        }
        finally
        {
            File.Delete(irPath);
        }
    }
}
