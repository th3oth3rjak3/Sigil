using Sigil.Compiler.Semantics;
using Sigil.Compiler.Syntax;

namespace Sigil.Compiler.Tests.Semantics;

public sealed class TypeCheckerTests
{
    [Fact]
    public void IntegerLiteralHasIntegerType()
    {
        var module = Parse("""
            fn main() -> Integer {
                return 42;
            }
            """);

        var bound = new NameResolver().Resolve(module);
        var typed = new TypeChecker().Check(bound);

        var function = Assert.Single(typed.Declarations);
        var statement = Assert.IsType<TypedReturnStatement>(
            Assert.Single(function.Body.Statements));

        Assert.NotNull(statement.Value);
        Assert.IsType<IntegerType>(statement.Value.Type);
    }

    [Fact]
    public void FunctionReturnTypeIsResolved()
    {
        var module = Parse("""
            fn main() -> Integer {
                return 42;
            }
            """);

        var bound = new NameResolver().Resolve(module);
        var typed = new TypeChecker().Check(bound);

        var function = Assert.Single(typed.Declarations);

        Assert.IsType<IntegerType>(function.ReturnType);
    }

    [Fact]
    public void LetInitializerHasDeclaredType()
    {
        var module = Parse("""
            fn main() -> Void {
                let x: Integer = 42;
            }
            """);

        var bound = new NameResolver().Resolve(module);
        var typed = new TypeChecker().Check(bound);

        var function = Assert.Single(typed.Declarations);
        var statement = Assert.IsType<TypedLetStatement>(
            Assert.Single(function.Body.Statements));

        Assert.IsType<IntegerType>(statement.Type);
        Assert.IsType<IntegerType>(statement.Initializer.Type);
    }

    [Fact]
    public void IntegerVariableReferenceHasIntegerType()
    {
        var module = Parse("""
            fn main() -> Integer {
                let x: Integer = 42;
                return x;
            }
            """);

        var bound = new NameResolver().Resolve(module);
        var typed = new TypeChecker().Check(bound);

        var function = Assert.Single(typed.Declarations);
        var statement = Assert.IsType<TypedReturnStatement>(
            Assert.Single(function.Body.Statements.Skip(1)));

        var identifier = Assert.IsType<TypedIdentifierExpression>(
            statement.Value);

        Assert.IsType<IntegerType>(identifier.Type);
    }

    [Fact]
    public void IntegerInitializerMatchesIntegerDeclaration()
    {
        var module = Parse("""
            fn main() -> Void {
                let x: Integer = 42;
            }
            """);

        var bound = new NameResolver().Resolve(module);

        var typed = new TypeChecker().Check(bound);

        Assert.NotNull(typed);
    }

    [Fact]
    public void IntegerReturnMatchesIntegerFunction()
    {
        var module = Parse("""
            fn main() -> Integer {
                return 42;
            }
            """);

        var bound = new NameResolver().Resolve(module);

        var typed = new TypeChecker().Check(bound);

        Assert.NotNull(typed);
    }

    [Fact]
    public void EmptyReturnIsValidInVoidFunction()
    {
        var module = Parse("""
            fn main() -> Void {
                return;
            }
            """);

        var bound = new NameResolver().Resolve(module);

        var typed = new TypeChecker().Check(bound);

        var function = Assert.Single(typed.Declarations);
        var statement = Assert.IsType<TypedReturnStatement>(
            Assert.Single(function.Body.Statements));

        Assert.Null(statement.Value);
    }

    [Fact]
    public void VoidFunctionMayHaveNoReturnStatement()
    {
        var module = Parse("""
            fn main() -> Void {
            }
            """);

        var bound = new NameResolver().Resolve(module);

        var typed = new TypeChecker().Check(bound);

        var function = Assert.Single(typed.Declarations);

        Assert.Empty(function.Body.Statements);
    }

    [Fact]
    public void IntegerReturnInVoidFunctionIsRejected()
    {
        var module = Parse("""
            fn main() -> Void {
                return 42;
            }
            """);

        var bound = new NameResolver().Resolve(module);

        Assert.Throws<Exception>(
            () => new TypeChecker().Check(bound));
    }

    [Fact]
    public void EmptyReturnInIntegerFunctionIsRejected()
    {
        var module = Parse("""
            fn main() -> Integer {
                return;
            }
            """);

        var bound = new NameResolver().Resolve(module);

        Assert.Throws<Exception>(
            () => new TypeChecker().Check(bound));
    }

    [Fact]
    public void IntegerInitializerInFloatVariableIsRejected()
    {
        var module = Parse("""
            fn main() -> Void {
                let x: Float = 42;
            }
            """);

        var bound = new NameResolver().Resolve(module);

        Assert.Throws<Exception>(
            () => new TypeChecker().Check(bound));
    }

    [Fact]
    public void UnknownVariableTypeIsRejected()
    {
        var module = Parse("""
            fn main() -> Void {
                let x: Nope = 42;
            }
            """);

        var bound = new NameResolver().Resolve(module);

        Assert.Throws<Exception>(
            () => new TypeChecker().Check(bound));
    }

    [Fact]
    public void UnknownFunctionReturnTypeIsRejected()
    {
        var module = Parse("""
            fn main() -> Nope {
                return 42;
            }
            """);

        var bound = new NameResolver().Resolve(module);

        Assert.Throws<Exception>(
            () => new TypeChecker().Check(bound));
    }

    private static Module Parse(string source)
    {
        var lexer = new Lexer(source);
        var parser = new Parser(lexer);

        return parser.Parse();
    }
}