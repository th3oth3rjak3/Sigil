using Sigil.Compiler.Semantics.Types;

namespace Sigil.Compiler.Semantics.TypedExpressions;

public abstract class TypedExpression(SigilType type)
{
    public SigilType Type { get; } = type;
}
