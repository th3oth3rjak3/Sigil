namespace Sigil.ModuleImporting;

public static class FileLoader
{
    public static string LoadSourceCode(string fileName)
    {
        using var fileStream = File.OpenText(fileName);
        var sourceCode = fileStream.ReadToEnd();
        return sourceCode;
    }
}
