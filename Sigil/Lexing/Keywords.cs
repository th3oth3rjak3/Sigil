namespace Sigil.Lexing;

public static class Keywords
{
    private static Dictionary<string, TokenType> _keywords = new() {
        {"let", TokenType.Let },
        {"fun", TokenType.Fun },
        {"class", TokenType.Class },
        {"new", TokenType.New },
        {"this", TokenType.This },
        {"if", TokenType.If },
        {"else", TokenType.Else },
        {"while", TokenType.While },
        {"for", TokenType.For },
        {"return", TokenType.Return },
        {"true", TokenType.True },
        {"false", TokenType.False },
        {"break", TokenType.Break },
        {"continue", TokenType.Continue },
        {"or", TokenType.Or },
        {"and", TokenType.And},
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
