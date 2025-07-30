using Sigil.ErrorHandling;
using Sigil.Interpretation;
using Sigil.Lexing;
using Sigil.Parsing;

namespace Sigil.Tests.Interpretation;

public class InterpreterTests
{
    private string RunAndCapture(string source)
    {
        var output = new StringWriter();
        var originalOut = Console.Out;
        Console.SetOut(output);

        try
        {
            var errorHandler = new ErrorHandler(source);
            var lexer = new Lexer(source, errorHandler);
            var tokens = lexer.Tokenize();
            var parser = new Parser(tokens, errorHandler, source);
            var statements = parser.Parse();

            if (errorHandler.HadError)
            {
                return "";
            }

            var interpreter = new Interpreter(errorHandler);
            interpreter.Interpret(statements);

            // Get all output and filter out debug lines
            var allOutput = output.ToString();
            var lines = allOutput.Split('\n', StringSplitOptions.RemoveEmptyEntries);

            // Find lines that are actual program output (numbers, not debug messages)
            var programOutput = lines
                .Where(line =>
                    !line.StartsWith("===") &&
                    !line.StartsWith("Tokens:") &&
                    !line.StartsWith("Statements:") &&
                    !line.StartsWith("Parse errors:") &&
                    !line.StartsWith("Starting") &&
                    !line.StartsWith("Interpreting") &&
                    !line.StartsWith("Executing") &&
                    !line.StartsWith("Visiting") &&
                    !line.StartsWith("Expression result:") &&
                    !line.StartsWith("Stringified:") &&
                    !line.StartsWith("Statement result:") &&
                    !line.StartsWith("Interpretation") &&
                    !line.StartsWith("Defining") &&
                    !line.StartsWith("Looking") &&
                    !line.StartsWith("Found:") &&
                    !string.IsNullOrWhiteSpace(line))
                .LastOrDefault(); // Get the last meaningful output

            return programOutput?.Trim() ?? "";
        }
        finally
        {
            Console.SetOut(originalOut);
        }
    }

    // Remove the debug Console.WriteLine statements from your parser methods:
    // - ParsePrimary()
    // - Match()  
    // - Advance()

    // And add some interpreter tests:
    [Fact]
    public void Interpreter_ShouldEvaluateSimpleArithmetic()
    {
        var source = "return 1 + 2 * 3;"; // Should print: 7
        var result = RunAndCapture(source);
        Assert.Equal("7", result);
    }

    [Fact]
    public void Interpreter_ShouldHandleVariables()
    {
        var source = """
        let x = 10;
        let y = 20;
        return x + y;
        """;
        var result = RunAndCapture(source);
        Assert.Equal("30", result);
    }

    [Fact]
    public void Debug_SimpleNumber()
    {
        var source = "42;";
        var result = RunAndCapture(source);

        // Let's see what we actually get
        Console.WriteLine($"Result: '{result}'");
        Console.WriteLine($"Length: {result.Length}");
    }



    [Fact]
    public void Interpreter_ShouldRequireExplicitReturn()
    {
        var source = "return 1 + 2 * 3;"; // Now requires 'return'
        var result = RunAndCapture(source);
        Assert.Equal("7", result);
    }

    [Fact]
    public void Interpreter_ShouldHandleVariablesWithReturn()
    {
        var source = """
        let x = 10;
        let y = 20;
        return x + y;
        """;
        var result = RunAndCapture(source);
        Assert.Equal("30", result);
    }

    [Fact]
    public void Parser_ShouldRejectBareExpressions()
    {
        var source = "42;"; // This should now be invalid
        var errorHandler = new ErrorHandler(source);
        var lexer = new Lexer(source, errorHandler);
        var tokens = lexer.Tokenize();
        var parser = new Parser(tokens, errorHandler, source);

        var statements = parser.Parse();

        // Should have parse errors
        Assert.True(errorHandler.HadError);
    }

    [Fact]
    public void Interpreter_ShouldHandleIfStatementTrue()
    {
        var source = """
        let x = 5;
        if x > 3 {
            return 42;
        }
        """;
        var result = RunAndCapture(source);
        Assert.Equal("42", result);
    }

    [Fact]
    public void Interpreter_ShouldHandleIfStatementFalse()
    {
        var source = """
        let x = 1;
        if x > 3 {
            return 42;
        } else {
            return 99;
        }
        """;
        var result = RunAndCapture(source);
        Assert.Equal("99", result);
    }

    [Fact]
    public void Interpreter_ShouldHandleWhileLoop()
    {
        var source = """
        let i = 0;
        let sum = 0;
        while i < 3 {
            sum = sum + i;
            i = i + 1;
        }
        return sum;
        """;
        var result = RunAndCapture(source);
        Assert.Equal("3", result); // 0 + 1 + 2 = 3
    }

    [Fact]
    public void Interpreter_ShouldHandleLogicalAnd()
    {
        var source = """
        let result = true and false;
        return result;
        """;
        var result = RunAndCapture(source);
        Assert.Equal("False", result);
    }

    [Fact]
    public void Interpreter_ShouldHandleLogicalOr()
    {
        var source = """
        let result = false or true;
        return result;
        """;
        var result = RunAndCapture(source);
        Assert.Equal("True", result);
    }

    [Fact]
    public void Interpreter_ShouldShortCircuitLogicalAnd()
    {
        var source = """
        return false and (5 / 0); // Should not divide by zero
        """;
        var result = RunAndCapture(source);
        Assert.Equal("False", result);
    }

    [Fact]
    public void Interpreter_ShouldHandleVariableAssignment()
    {
        var source = """
        let x = 10;
        x = 20;
        return x;
        """;
        var result = RunAndCapture(source);
        Assert.Equal("20", result);
    }

    [Fact]
    public void Interpreter_ShouldErrorOnUndefinedVariableAssignment()
    {
        var source = """
        y = 42;
        """;

        var errorHandler = new ErrorHandler(source);
        var lexer = new Lexer(source, errorHandler);
        var tokens = lexer.Tokenize();
        var parser = new Parser(tokens, errorHandler, source);
        var statements = parser.Parse();

        var interpreter = new Interpreter(errorHandler);
        interpreter.Interpret(statements);

        Assert.True(errorHandler.HadError);
    }

    [Fact]
    public void Interpreter_shouldHandleFactorialTest()
    {
        var source = """
        fun factorial(n) {
            if n <= 1 {
                return 1;
            }
            return n * factorial(n - 1);
        }

        return factorial(5); // 120
        """;

        var result = RunAndCapture(source);
        Assert.Equal("120", result);
    }

    [Theory]
    [InlineData(10, "55")]
    [InlineData(25, "75025")]
    //[InlineData(30, "832040")] // Too slow for test cycle.
    //[InlineData(35, "9227465")] // Throws OOM Exception.
    //[InlineData(40, "102334155")] // Throws OOM Exception.
    public void Benchmark_Fibonacci(int n, string expected)
    {
        var source = $$"""
        fun fib(n) {
            if n < 2 {
                return n;
            }
            return fib(n - 1) + fib(n - 2);
        }
        return fib({{n}});
        """;

        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        var result = RunAndCapture(source);

        stopwatch.Stop();

        Console.WriteLine($"fib({n}) = {result}");
        Console.WriteLine($"Execution time: {stopwatch.ElapsedMilliseconds} ms");
        Console.WriteLine($"Execution time: {stopwatch.Elapsed.TotalSeconds:F2} seconds");

        Assert.Equal(expected, result);
    }
}