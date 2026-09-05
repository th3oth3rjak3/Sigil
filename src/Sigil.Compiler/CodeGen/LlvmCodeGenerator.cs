using LLVMSharp.Interop;
using Sigil.Compiler.Semantics;
using Sigil.Compiler.Syntax;

namespace Sigil.Compiler.CodeGen;

public sealed class LlvmCodeGenerator
{
    public string Generate(TypedModule module)
    {
        using var context = LLVMContextRef.Create();
        using var llvmModule = context.CreateModuleWithName("sigil");

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
                builder,
                locals,
                statement);
        }
    }

    private static void GenerateStatement(
        LLVMContextRef context,
        LLVMBuilderRef builder,
        Dictionary<Declaration, LLVMValueRef> locals,
        TypedStatement statement)
    {
        if (statement is TypedLetStatement let)
        {
            var value = GenerateExpression(
                context,
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
                builder,
                locals,
                binary.Left);

            var right = GenerateExpression(
                context,
                builder,
                locals,
                binary.Right);

            return builder.BuildAdd(
                left,
                right,
                "add");
        }

        throw new Exception(
            $"Unsupported typed expression: {expression.GetType().Name}.");
    }

    private static LLVMTypeRef GetLlvmType(
        LLVMContextRef context,
        Semantics.Type type)
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