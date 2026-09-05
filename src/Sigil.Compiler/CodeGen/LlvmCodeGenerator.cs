using LLVMSharp.Interop;
using Sigil.Compiler.Semantics.BoundExpressions;
using Sigil.Compiler.Semantics.TypedDeclarations;
using Sigil.Compiler.Semantics.TypedExpressions;
using Sigil.Compiler.Semantics.TypedPrimitives;
using Sigil.Compiler.Semantics.TypedStatements;
using Sigil.Compiler.Semantics.Types;
using Sigil.Compiler.Syntax.Declarations;
using Sigil.Compiler.Syntax.Primitives;

namespace Sigil.Compiler.CodeGen;

public sealed class LlvmCodeGenerator
{
    public string Generate(TypedModule module)
    {
        using var context = LLVMContextRef.Create();
        using var llvmModule = context.CreateModuleWithName("sigil");

        DeclareRuntimeFunctions(context, llvmModule);

        foreach (var function in module.Declarations)
        {
            GenerateFunction(context, llvmModule, function);
        }

        return llvmModule.PrintToString();
    }

    private static void GenerateFunction(
        LLVMContextRef context,
        LLVMModuleRef module,
        TypedFunctionDeclaration function)
    {
        var returnType = GetLlvmType(
            context,
            function.ReturnType);

        var functionType = LLVMTypeRef.CreateFunction(
            returnType,
            Array.Empty<LLVMTypeRef>(),
            false);

        var llvmFunction = module.AddFunction(
            function.Declaration.Name,
            functionType);

        var entry = llvmFunction.AppendBasicBlock("entry");

        var builder = context.CreateBuilder();

        builder.PositionAtEnd(entry);

        var locals =
            new Dictionary<Declaration, LLVMValueRef>();

        foreach (var statement in function.Body.Statements)
        {
            GenerateStatement(
                context,
                module,
                builder,
                locals,
                statement);
        }
    }

    private static void DeclareRuntimeFunctions(
    LLVMContextRef context,
    LLVMModuleRef module)
    {
        var integerType = LLVMTypeRef.CreateFunction(
            context.VoidType,
            [context.Int64Type],
            false);

        module.AddFunction(
            "sigil_println_integer",
            integerType);

        var floatType = LLVMTypeRef.CreateFunction(
            context.VoidType,
            [context.DoubleType],
            false);

        module.AddFunction(
            "sigil_println_float",
            floatType);
    }

    private static LLVMValueRef GenerateCallExpression(
        LLVMContextRef context,
        LLVMModuleRef module,
        LLVMBuilderRef builder,
        Dictionary<Declaration, LLVMValueRef> locals,
        TypedCallExpression expression)
    {
        var arguments = expression.Arguments
            .Select(argument => GenerateExpression(
                context,
                module,
                builder,
                locals,
                argument))
            .ToArray();

        var functionType = LLVMTypeRef.CreateFunction(
            GetLlvmType(context, expression.Type),
            expression.Arguments
                .Select(argument => GetLlvmType(context, argument.Type))
                .ToArray(),
            false);

        LLVMValueRef function;

        switch (expression.Expression.Callee)
        {
            case BoundIdentifierExpression identifier:
                function = module.GetNamedFunction(
                    identifier.Symbol.Name);
                break;

            // TODO: figure out if this is the right place for builtins.

            default:
                throw new Exception(
                    $"Unsupported call target: " +
                    $"{expression.Expression.Callee.GetType().Name}.");
        }

        return builder.BuildCall2(
            functionType,
            function,
            arguments,
            "");
    }

    private static void GenerateStatement(
        LLVMContextRef context,
        LLVMModuleRef module,
        LLVMBuilderRef builder,
        Dictionary<Declaration, LLVMValueRef> locals,
        TypedStatement statement)
    {
        if (statement is TypedLetStatement let)
        {
            var value = GenerateExpression(
                context,
                module,
                builder,
                locals,
                let.Initializer);

            var storage = builder.BuildAlloca(
                GetLlvmType(context, let.Type),
                let.Variable.Name);

            builder.BuildStore(
                value,
                storage);

            locals.Add(
                let.Variable,
                storage);

            return;
        }

        if (statement is TypedReturnStatement returnStatement)
        {
            if (returnStatement.Value is null)
            {
                builder.BuildRetVoid();
                return;
            }

            var value = GenerateExpression(
                context,
                module,
                builder,
                locals,
                returnStatement.Value);

            builder.BuildRet(value);
            return;
        }

        throw new Exception(
            $"Unsupported typed statement: {statement.GetType().Name}.");
    }

    private static LLVMValueRef GenerateExpression(
        LLVMContextRef context,
        LLVMModuleRef module,
        LLVMBuilderRef builder,
        Dictionary<Declaration, LLVMValueRef> locals,
        TypedExpression expression)
    {
        if (expression is TypedIntegerLiteralExpression integer)
        {
            return LLVMValueRef.CreateConstInt(
                context.Int64Type,
                (ulong)integer.Expression.Value,
                true);
        }

        if (expression is TypedFloatLiteralExpression floatLiteral)
        {
            return LLVMValueRef.CreateConstReal(
                context.DoubleType,
                floatLiteral.Expression.Value);
        }

        if (expression is TypedIdentifierExpression identifier)
        {
            var storage = locals[identifier.Symbol.Declaration];

            return builder.BuildLoad2(
                GetLlvmType(context, identifier.Type),
                storage,
                identifier.Symbol.Name);
        }

        if (expression is TypedBinaryExpression binary)
        {
            var left = GenerateExpression(
                context,
                module,
                builder,
                locals,
                binary.Left);

            var right = GenerateExpression(
                context,
                module,
                builder,
                locals,
                binary.Right);

            return binary.Type switch
            {
                IntegerType => GenerateIntegerBinaryExpression(
                    builder,
                    binary.Expression.OperatorKind,
                    left,
                    right),

                FloatType => GenerateFloatBinaryExpression(
                    builder,
                    binary.Expression.OperatorKind,
                    left,
                    right),

                _ => throw new Exception(
                    $"Unsupported binary type: " +
                    $"{binary.Type.GetType().Name}.")
            };
        }

        if (expression is TypedCallExpression call)
        {
            return GenerateCallExpression(
                context,
                module,
                builder,
                locals,
                call);
        }

        throw new Exception(
            $"Unsupported typed expression: {expression.GetType().Name}.");
    }

    private static LLVMValueRef GenerateIntegerBinaryExpression(
    LLVMBuilderRef builder,
    TokenKind operatorKind,
    LLVMValueRef left,
    LLVMValueRef right)
    {
        return operatorKind switch
        {
            TokenKind.Plus => builder.BuildAdd(left, right, "add"),
            TokenKind.Minus => builder.BuildSub(left, right, "sub"),
            TokenKind.Star => builder.BuildMul(left, right, "mul"),
            TokenKind.Slash => builder.BuildSDiv(left, right, "div"),

            _ => throw new Exception(
                $"Unsupported integer binary operator: {operatorKind}.")
        };
    }

    private static LLVMValueRef GenerateFloatBinaryExpression(
        LLVMBuilderRef builder,
        TokenKind operatorKind,
        LLVMValueRef left,
        LLVMValueRef right)
    {
        return operatorKind switch
        {
            TokenKind.Plus => builder.BuildFAdd(left, right, "add"),
            TokenKind.Minus => builder.BuildFSub(left, right, "sub"),
            TokenKind.Star => builder.BuildFMul(left, right, "mul"),
            TokenKind.Slash => builder.BuildFDiv(left, right, "div"),

            _ => throw new Exception(
                $"Unsupported float binary operator: {operatorKind}.")
        };
    }

    private static LLVMTypeRef GetLlvmType(
        LLVMContextRef context,
        Semantics.Types.SigilType type)
    {
        return type switch
        {
            IntegerType => context.Int64Type,
            FloatType => context.DoubleType,
            BooleanType => context.Int1Type,
            StringType => LLVMTypeRef.CreatePointer(context.Int8Type, 0),
            VoidType => context.VoidType,

            _ => throw new Exception(
                $"Unsupported type: {type.GetType().Name}.")
        };
    }
}
