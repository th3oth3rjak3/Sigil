using Sigil.Compiler;

if (args.Length != 3 || args[1] != "-o")
{
    Console.Error.WriteLine(
        "Usage: sigil <source.sgl> -o <output>");

    return 1;
}

var sourcePath = args[0];
var outputPath = args[2];

if (!File.Exists(sourcePath))
{
    Console.Error.WriteLine(
        $"Source file not found: {sourcePath}");

    return 1;
}

try
{
    var source = File.ReadAllText(sourcePath);

    new Compiler().Compile(
        source,
        outputPath);

    return 0;
}
catch (Exception ex)
{
    Console.Error.WriteLine(ex.Message);

    return 1;
}
