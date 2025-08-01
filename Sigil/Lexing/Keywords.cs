namespace Sigil.Lexing;

public static class Keywords
{
    private static Dictionary<string, TokenType> _keywords = new() {
        {"break", TokenType.Break },
        {"class", TokenType.Class },
        {"else", TokenType.Else },
        {"false", TokenType.False },
        {"for", TokenType.For },
        {"fun", TokenType.Fun },
        {"if", TokenType.If },
        {"let", TokenType.Let },
        {"new", TokenType.New },
        {"this", TokenType.This },
        {"while", TokenType.While },
        {"return", TokenType.Return },
        {"true", TokenType.True },
        {"continue", TokenType.Continue },
        {"or", TokenType.Or },
        {"and", TokenType.And},
        {"print", TokenType.Print}
    };

    public static Option<TokenType> FromString(string input)
    {
        if (_keywords.TryGetValue(input, out var tokenType))
        {
            return Some(tokenType);
        }

        return None<TokenType>();
    }
}
