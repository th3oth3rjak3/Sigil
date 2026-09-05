using LLVMSharp.Interop;
using Sigil.Compiler.Semantics;

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

        foreach (var statement in function.Body.Statements)
        {
            GenerateStatement(
                context,
                builder,
                statement);
        }
    }

    private static void GenerateStatement(
        LLVMContextRef context,
        LLVMBuilderRef builder,
        TypedStatement statement)
    {
        if (statement is TypedReturnStatement returnStatement)
        {
            if (returnStatement.Value is null)
            {
                builder.BuildRetVoid();
                return;
            }

            var value = GenerateExpression(
                context,
                returnStatement.Value);

            builder.BuildRet(value);
            return;
        }

        throw new Exception(
            $"Unsupported typed statement: {statement.GetType().Name}.");
    }

    private static LLVMValueRef GenerateExpression(
        LLVMContextRef context,
        TypedExpression expression)
    {
        if (expression is TypedIntegerLiteralExpression integer)
        {
            return LLVMValueRef.CreateConstInt(
                context.Int64Type,
                (ulong)integer.Expression.Value,
                true);
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

            _ => throw new Exception($"Unsupported type: {type.GetType().Name}.")
        };
    }
}