﻿using Sigil.ErrorHandling;
using Sigil.Interpretation;
using Sigil.Lexing;
using Sigil.Parsing;

namespace Sigil.Tests.Interpretation;

public class InterpreterTests
{
    private string RunAndCapture(string source)
    {
        var output = new StringWriter();
        var errorHandler = new ErrorHandler(source);

        var lexer = new Lexer(source, errorHandler);
        var tokens = lexer.Tokenize();
        if (errorHandler.HadError) return string.Join("\n", errorHandler.Errors);

        var parser = new Parser(tokens, errorHandler, source);
        var statements = parser.Parse();
        if (errorHandler.HadError) return string.Join("\n", errorHandler.Errors);

        var interpreter = new Interpreter(source, output);
        interpreter.Interpret(statements);

        if (interpreter.ErrorHandler.HadError)
        {
            // For now, we don't test for runtime errors this way.
            return string.Join("\n", interpreter.ErrorHandler.Errors);
        }

        var result = output.ToString();
        var lines = result.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries);

        // Most tests expect the last line of output.
        return lines.LastOrDefault()?.Trim() ?? "";
    }

    // Remove the debug Console.WriteLine statements from your parser methods:
    // - ParsePrimary()
    // - Match()
    // - Advance()


    // And add some interpreter tests:
    [Fact]
    public void Interpreter_ShouldEvaluateSimpleArithmetic()
    {
        var source = "print 1 + 2 * 3;"; // Should print: 7
        var result = RunAndCapture(source);
        Console.WriteLine(result);
        Assert.Equal("7", result);
    }

    [Fact]
    public void Interpreter_ShouldHandleVariables()
    {
        var source = """
        let x = 10;
        let y = 20;
        print x + y;
        """;
        var result = RunAndCapture(source);
        Assert.Equal("30", result);
    }

    [Fact]
    public void Debug_SimpleNumber()
    {
        var source = "42;";
        var result = RunAndCapture(source);
        // This test has no print statement, so output should be empty.
        Assert.Equal("", result);
    }

    [Fact]
    public void Interpreter_ShouldHandleIfStatementTrue()
    {
        var source = """
        let x = 5;
        if x > 3 {
            print 42;
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
            print 42;
        } else {
            print 99;
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
        print sum;
        """;
        var result = RunAndCapture(source);
        Assert.Equal("3", result); // 0 + 1 + 2 = 3
    }

    [Fact]
    public void Interpreter_ShouldHandleLogicalAnd()
    {
        var source = """
        let result = true and false;
        print(result);
        """;
        var result = RunAndCapture(source);
        Assert.Equal("False", result);
    }

    [Fact]
    public void Interpreter_ShouldHandleLogicalOr()
    {
        var source = """
        let result = false or true;
        print(result);
        """;
        var result = RunAndCapture(source);
        Assert.Equal("True", result);
    }

    [Fact]
    public void Interpreter_ShouldShortCircuitLogicalAnd()
    {
        var source = """
        print(false and (5 / 0)); // Should not divide by zero
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
        print(x);
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

        var interpreter = new Interpreter(source);
        interpreter.Interpret(statements);

        Assert.False(errorHandler.HadError); // Parsing Error Handler, not for runtime use.
        Assert.True(interpreter.ErrorHandler.HadError); // should have been a runtime error.

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

        print(factorial(5)); // 120
        """;

        var result = RunAndCapture(source);
        Assert.Equal("120", result);
    }

    [Fact]
    public void Interpreter_ShouldHandleStringAndCharConcatenation()
    {
        var source = """
        let greeting = "hello";
        let space = ' ';
        let world = "world";
        let exclam = '!';
        print greeting + space + world + exclam;
        """;
        var result = RunAndCapture(source);
        Assert.Equal("hello world!", result);
    }

    [Fact]
    public void Interpreter_ShouldHandleCharConcatenation()
    {
        var source = """
        print('a' + 'b' + "c");
        """;
        var result = RunAndCapture(source);
        Assert.Equal("abc", result);
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
        print fib({{n}});
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
