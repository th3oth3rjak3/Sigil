using Sigil.Compiler.Semantics;
using Sigil.Compiler.Semantics.Primitives;
using Sigil.Compiler.Semantics.TypedDeclarations;
using Sigil.Compiler.Semantics.TypedExpressions;
using Sigil.Compiler.Semantics.TypedPrimitives;
using Sigil.Compiler.Semantics.TypedStatements;
using Sigil.Compiler.Semantics.Types;
using Sigil.Compiler.Syntax;
using Sigil.Compiler.Syntax.Primitives;

namespace Sigil.Compiler.Tests.Semantics;

public sealed class TypeCheckerTests
{
    private static TypedModule TypeCheck(string source)
    {
        var module = new Parser(new Lexer(source)).Parse();
        var bound = new NameResolver(new BuiltinRegistry()).Resolve(module);

        return new TypeChecker().Check(bound);
    }

    private static TypedFunctionDeclaration GetFunction(TypedModule module)
    {
        return Assert.Single(module.Declarations);
    }

    private static TypedReturnStatement GetReturnStatement(
        TypedFunctionDeclaration function,
        int index = 0)
    {
        return Assert.IsType<TypedReturnStatement>(
            function.Body.Statements[index]);
    }

    private static TypedLetStatement GetLetStatement(
        TypedFunctionDeclaration function,
        int index = 0)
    {
        return Assert.IsType<TypedLetStatement>(
            function.Body.Statements[index]);
    }

    private static TypedBinaryExpression GetBinaryExpression(
        TypedReturnStatement statement)
    {
        return Assert.IsType<TypedBinaryExpression>(
            statement.Value);
    }

    [Fact]
    public void IntegerLiteralHasIntegerType()
    {
        var typed = TypeCheck("""
            fn main() -> Integer {
                return 42;
            }
            """);

        var function = GetFunction(typed);
        var statement = GetReturnStatement(function);

        Assert.NotNull(statement.Value);
        Assert.IsType<IntegerType>(statement.Value.Type);
    }

    [Fact]
    public void FunctionReturnTypeIsResolved()
    {
        var typed = TypeCheck("""
            fn main() -> Integer {
                return 42;
            }
            """);

        var function = GetFunction(typed);

        Assert.IsType<IntegerType>(function.ReturnType);
    }

    [Fact]
    public void LetInitializerHasDeclaredType()
    {
        var typed = TypeCheck("""
            fn main() -> Void {
                let x: Integer = 42;
            }
            """);

        var function = GetFunction(typed);
        var statement = GetLetStatement(function);

        Assert.IsType<IntegerType>(statement.Type);
        Assert.IsType<IntegerType>(statement.Initializer.Type);
    }

    [Fact]
    public void IntegerVariableReferenceHasIntegerType()
    {
        var typed = TypeCheck("""
            fn main() -> Integer {
                let x: Integer = 42;
                return x;
            }
            """);

        var function = GetFunction(typed);
        var statement = GetReturnStatement(function, 1);

        var identifier = Assert.IsType<TypedIdentifierExpression>(
            statement.Value);

        Assert.IsType<IntegerType>(identifier.Type);
    }

    [Fact]
    public void IntegerInitializerMatchesIntegerDeclaration()
    {
        var typed = TypeCheck("""
            fn main() -> Void {
                let x: Integer = 42;
            }
            """);

        Assert.NotNull(typed);
    }

    [Fact]
    public void IntegerReturnMatchesIntegerFunction()
    {
        var typed = TypeCheck("""
            fn main() -> Integer {
                return 42;
            }
            """);

        Assert.NotNull(typed);
    }

    [Fact]
    public void EmptyReturnIsValidInVoidFunction()
    {
        var typed = TypeCheck("""
            fn main() -> Void {
                return;
            }
            """);

        var function = GetFunction(typed);
        var statement = GetReturnStatement(function);

        Assert.Null(statement.Value);
    }

    [Fact]
    public void VoidFunctionMayHaveNoReturnStatement()
    {
        var typed = TypeCheck("""
            fn main() -> Void {
            }
            """);

        var function = GetFunction(typed);

        Assert.Empty(function.Body.Statements);
    }

    [Fact]
    public void IntegerReturnInVoidFunctionIsRejected()
    {
        var module = new Parser(new Lexer("""
            fn main() -> Void {
                return 42;
            }
            """)).Parse();

        var bound = new NameResolver(new BuiltinRegistry()).Resolve(module);

        Assert.Throws<Exception>(
            () => new TypeChecker().Check(bound));
    }

    [Fact]
    public void EmptyReturnInIntegerFunctionIsRejected()
    {
        var module = new Parser(new Lexer("""
            fn main() -> Integer {
                return;
            }
            """)).Parse();

        var bound = new NameResolver(new BuiltinRegistry()).Resolve(module);

        Assert.Throws<Exception>(
            () => new TypeChecker().Check(bound));
    }

    [Fact]
    public void IntegerInitializerInFloatVariableIsRejected()
    {
        var module = new Parser(new Lexer("""
            fn main() -> Void {
                let x: Float = 42;
            }
            """)).Parse();

        var bound = new NameResolver(new BuiltinRegistry()).Resolve(module);

        Assert.Throws<Exception>(
            () => new TypeChecker().Check(bound));
    }

    [Fact]
    public void UnknownVariableTypeIsRejected()
    {
        var module = new Parser(new Lexer("""
            fn main() -> Void {
                let x: Nope = 42;
            }
            """)).Parse();

        var bound = new NameResolver(new BuiltinRegistry()).Resolve(module);

        Assert.Throws<Exception>(
            () => new TypeChecker().Check(bound));
    }

    [Fact]
    public void UnknownFunctionReturnTypeIsRejected()
    {
        var module = new Parser(new Lexer("""
            fn main() -> Nope {
                return 42;
            }
            """)).Parse();

        var bound = new NameResolver(new BuiltinRegistry()).Resolve(module);

        Assert.Throws<Exception>(
            () => new TypeChecker().Check(bound));
    }

    [Fact]
    public void AdditionExpressionHasIntegerType()
    {
        var typed = TypeCheck("""
            fn main() -> Integer {
                return 20 + 22;
            }
            """);

        var function = GetFunction(typed);
        var statement = GetReturnStatement(function);
        var expression = GetBinaryExpression(statement);

        Assert.IsType<IntegerType>(expression.Type);
        Assert.IsType<TypedIntegerLiteralExpression>(expression.Left);
        Assert.IsType<TypedIntegerLiteralExpression>(expression.Right);
    }

    [Fact]
    public void MultiplicationExpressionHasIntegerType()
    {
        var typed = TypeCheck("""
            fn main() -> Integer {
                return 20 * 22;
            }
            """);

        var function = GetFunction(typed);
        var statement = GetReturnStatement(function);
        var expression = GetBinaryExpression(statement);

        Assert.IsType<IntegerType>(expression.Type);
        Assert.IsType<TypedIntegerLiteralExpression>(expression.Left);
        Assert.IsType<TypedIntegerLiteralExpression>(expression.Right);
    }

    [Fact]
    public void ChecksFloatLiteralExpression()
    {
        var typed = TypeCheck("""
            fn main() -> Float {
                return 42.5;
            }
            """);

        var function = GetFunction(typed);
        var statement = GetReturnStatement(function);

        var expression = Assert.IsType<TypedFloatLiteralExpression>(
            statement.Value);

        Assert.IsType<FloatType>(expression.Type);
    }

    [Fact]
    public void ChecksFloatDivisionExpression()
    {
        var typed = TypeCheck("""
            fn main() -> Float {
                return 42.0 / 2.0;
            }
            """);

        var function = GetFunction(typed);
        var statement = GetReturnStatement(function);
        var expression = GetBinaryExpression(statement);

        Assert.Equal(TokenKind.Slash, expression.Expression.OperatorKind);
        Assert.IsType<FloatType>(expression.Type);
        Assert.IsType<TypedFloatLiteralExpression>(expression.Left);
        Assert.IsType<TypedFloatLiteralExpression>(expression.Right);
    }

    [Fact]
    public void ChecksCallExpression()
    {
        var typed = TypeCheck("""
        fn add(a: Integer, b: Integer) -> Integer {
            return a + b;
        }

        fn main() -> Integer {
            return add(20, 22);
        }
        """);

        var main = Assert.IsType<TypedFunctionDeclaration>(
            typed.Declarations[1]);

        var statement = Assert.IsType<TypedReturnStatement>(
            Assert.Single(main.Body.Statements));

        var call = Assert.IsType<TypedCallExpression>(
            statement.Value);

        Assert.IsType<IntegerType>(call.Type);
        Assert.Equal(2, call.Arguments.Count);

        Assert.IsType<IntegerType>(call.Arguments[0].Type);
        Assert.IsType<IntegerType>(call.Arguments[1].Type);
    }
}
