using Sigil.Common;
using Sigil.ErrorHandling;
using Sigil.Lexing;
using Sigil.Parsing.Expressions;
using Sigil.Parsing.Statements;

namespace Sigil.Parsing;

public class Parser(List<Token> Tokens, ErrorHandler ErrorHandler, string SourceCode)
{
    private int _current = 0;

    private Token Peek() => _current >= Tokens.Count ? Tokens.Last() : Tokens[_current];
    private Token Previous() => Tokens[_current - 1];
    private bool IsAtEnd() => Peek().TokenType == TokenType.Eof;

    private Token Advance()
    {
        if (!IsAtEnd()) _current++;
        return Previous();
    }

    private bool Check(TokenType type)
    {
        if (IsAtEnd()) return false;
        return Peek().TokenType == type;
    }

    private bool Match(params TokenType[] types)
    {
        foreach (var type in types)
        {
            if (Check(type))
            {
                Advance();
                return true;
            }
        }
        return false;
    }

    private Option<Token> TryConsume(TokenType type, string message)
    {
        if (Check(type)) return Some(Advance());

        ErrorHandler.Report(message, Peek().Span);
        return None<Token>();
    }

    // Main parsing entry point
    public List<Statement> Parse()
    {
        var statements = new List<Statement>();

        while (!IsAtEnd())
        {
            var stmt = ParseStatement();
            stmt.EffectSome(statements.Add);

            if (stmt.IsNone)
            {
                Synchronize();
            }
        }

        return statements;
    }

    // Error recovery - advance until we find a likely statement boundary
    private void Synchronize()
    {
        Advance();

        while (!IsAtEnd())
        {
            if (Previous().TokenType == TokenType.Semicolon) return;

            if (Peek().TokenType is TokenType.Class or TokenType.Fun or TokenType.Let
                or TokenType.For or TokenType.If or TokenType.While or TokenType.Return)
                return;

            Advance();
        }
    }

    // Statement parsing - returns None on error
    private Option<Statement> ParseStatement()
    {
        if (Match(TokenType.Let)) return ParseLetStatement();
        if (Match(TokenType.Return)) return ParseReturnStatement();
        if (Match(TokenType.Fun)) return ParseFunctionStatement();
        if (Match(TokenType.If)) return ParseIfStatement();
        if (Match(TokenType.While)) return ParseWhileStatement();
        if (Match(TokenType.LeftBrace)) return ParseBlockStatement();
        if (Match(TokenType.Print)) return ParsePrintStatement();

        if (Check(TokenType.Identifier))
        {
            var assignmentResult = TryParseAssignmentStatement();
            if (assignmentResult.IsSome) return assignmentResult;
        }

        var exprResult = ParseExpression();
        if (exprResult.IsSome)
        {
            var expr = exprResult.Unwrap();
            var semicolon = TryConsume(TokenType.Semicolon, "Expected ';' after expression.");
            var endPos = semicolon.Match(some => some.Span.End, () => expr.Span.End);
            var span = new Span(expr.Span.Start, endPos);
            return Some<Statement>(new ExpressionStatement(expr, span));
        }

        ErrorHandler.Report("Expected statement (let, return, or block).", Peek().Span);
        return None<Statement>();
    }

    private Option<Statement> ParseLetStatement()
    {
        var start = Previous().Span.Start;

        var nameToken = TryConsume(TokenType.Identifier, "Expected variable name.");
        if (nameToken.IsNone) return None<Statement>();

        var name = nameToken.Unwrap().Span.Slice(SourceCode);

        // Require the '=' token
        if (!TryConsume(TokenType.Equal, "Expected '=' after variable name.").IsSome)
            return None<Statement>();

        // Require an initializer expression
        var initExpr = ParseExpression();
        if (initExpr.IsNone) return None<Statement>();

        var initializer = initExpr.Unwrap();

        var semicolon = TryConsume(TokenType.Semicolon, "Expected ';' after variable declaration.");
        var endPos = semicolon.Match(
            some => some.Span.End,
            () => initializer.Span.End
        );

        var span = new Span(start, endPos);
        return Some<Statement>(new LetStatement(name, initializer, span));
    }

    private Option<Statement> TryParseAssignmentStatement()
    {
        var start = _current;

        var nameToken = TryConsume(TokenType.Identifier, "Expected variable name.");
        if (nameToken.IsNone)
        {
            _current = start;
            return None<Statement>();
        }

        if (!Check(TokenType.Equal))
        {
            _current = start;
            return None<Statement>();
        }

        Advance(); // Consume '='

        var name = nameToken.Unwrap().Span.Slice(SourceCode);
        var valueResult = ParseExpression();
        if (valueResult.IsNone) return None<Statement>();

        var value = valueResult.Unwrap();
        var semicolon = TryConsume(TokenType.Semicolon, "Expected ';' after assignment.");

        var endPos = semicolon.Match(
            some => some.Span.End,
            () => value.Span.End
        );

        var span = new Span(nameToken.Unwrap().Span.Start, endPos);
        return Some<Statement>(new AssignmentStatement(name, value, span));
    }

    private Option<Statement> ParseBlockStatement()
    {
        var start = Previous().Span.Start;
        var statements = new List<Statement>();

        while (!Check(TokenType.RightBrace) && !IsAtEnd())
        {
            var stmt = ParseStatement();
            stmt.EffectSome(statements.Add);

            // Continue parsing even if one statement failed
        }

        var endBrace = TryConsume(TokenType.RightBrace, "Expected '}' after block.");
        var endPos = endBrace.Match(
            some => some.Span.End,
            () => statements.LastOrDefault()?.Span.End ?? start // Fallback position
        );

        var span = new Span(start, endPos);
        return Some<Statement>(new BlockStatement(statements, span));
    }

    private Option<Statement> ParsePrintStatement()
    {
        var start = Previous().Span.Start;
        var exprResult = ParseExpression();
        if (exprResult.IsNone) return None<Statement>();

        var expression = exprResult.Unwrap();
        var semicolon = TryConsume(TokenType.Semicolon, "Expected ';' after print statement.");

        var endPos = semicolon.Match(
            some => some.Span.End,
            () => expression.Span.End
        );

        var span = new Span(start, endPos);
        return Some<Statement>(new PrintStatement(expression, span));
    }

    private Option<Statement> ParseReturnStatement()
    {
        var start = Previous().Span.Start;

        var exprResult = ParseExpression();
        if (exprResult.IsNone) return None<Statement>();

        var expression = exprResult.Unwrap();
        var semicolon = TryConsume(TokenType.Semicolon, "Expected ';' after return statement.");

        var endPos = semicolon.Match(
            some => some.Span.End,
            () => expression.Span.End
        );

        var span = new Span(start, endPos);
        return Some<Statement>(new ReturnStatement(expression, span));
    }

    // Add to your Parser.cs:
    private Option<Statement> ParseFunctionStatement()
    {
        var start = Previous().Span.Start;

        var nameToken = TryConsume(TokenType.Identifier, "Expected function name.");
        if (nameToken.IsNone) return None<Statement>();

        var name = nameToken.Unwrap().Span.Slice(SourceCode);

        if (!TryConsume(TokenType.LeftParen, "Expected '(' after function name.").IsSome)
            return None<Statement>();

        var parameters = new List<string>();
        if (!Check(TokenType.RightParen))
        {
            do
            {
                var paramToken = TryConsume(TokenType.Identifier, "Expected parameter name.");
                if (paramToken.IsNone) return None<Statement>();
                parameters.Add(paramToken.Unwrap().Span.Slice(SourceCode));
            } while (Match(TokenType.Comma));
        }

        if (!TryConsume(TokenType.RightParen, "Expected ')' after parameters.").IsSome)
            return None<Statement>();

        if (!TryConsume(TokenType.LeftBrace, "Expected '{' before function body.").IsSome)
            return None<Statement>();

        var body = new List<Statement>();
        while (!Check(TokenType.RightBrace) && !IsAtEnd())
        {
            var stmt = ParseStatement();
            stmt.EffectSome(body.Add);
        }

        var endBrace = TryConsume(TokenType.RightBrace, "Expected '}' after function body.");
        var endPos = endBrace.Match(some => some.Span.End, () => start);

        var span = new Span(start, endPos);
        return Some<Statement>(new FunctionStatement(name, parameters, body, span));
    }

    private Option<Statement> ParseIfStatement()
    {
        var start = Previous().Span.Start;

        // Parse condition
        var conditionResult = ParseExpression();
        if (conditionResult.IsNone) return None<Statement>();
        var condition = conditionResult.Unwrap();

        // Parse then branch
        var thenResult = ParseStatement();
        if (thenResult.IsNone) return None<Statement>();
        var thenBranch = thenResult.Unwrap();

        // Parse optional else branch
        Statement? elseBranch = null;
        if (Match(TokenType.Else))
        {
            var elseResult = ParseStatement();
            if (elseResult.IsNone) return None<Statement>();
            elseBranch = elseResult.Unwrap();
        }

        var endPos = elseBranch?.Span.End ?? thenBranch.Span.End;
        var span = new Span(start, endPos);

        return Some<Statement>(new IfStatement(condition, thenBranch, elseBranch, span));
    }

    private Option<Statement> ParseWhileStatement()
    {
        var start = Previous().Span.Start;

        // Parse condition
        var conditionResult = ParseExpression();
        if (conditionResult.IsNone) return None<Statement>();
        var condition = conditionResult.Unwrap();

        // Parse body
        var bodyResult = ParseStatement();
        if (bodyResult.IsNone) return None<Statement>();
        var body = bodyResult.Unwrap();

        var span = new Span(start, body.Span.End);
        return Some<Statement>(new WhileStatement(condition, body, span));
    }

    // Expression parsing - returns None on error
    private Option<Expression> ParseExpression() => ParseLogicalOr();

    private Option<Expression> ParseLogicalOr()
    {
        var expr = ParseLogicalAnd();
        if (expr.IsNone) return None<Expression>();

        var result = expr.Unwrap();

        while (Match(TokenType.Or)) // ||
        {
            var op = Previous();
            var rightResult = ParseLogicalAnd();
            if (rightResult.IsNone)
            {
                ErrorHandler.Report("Expected expression after operator.", op.Span);
                return Some(result);
            }

            var right = rightResult.Unwrap();
            var span = new Span(result.Span.Start, right.Span.End);
            result = new BinaryExpression(result, op, right, span);
        }

        return Some(result);
    }

    // Add this method to your Parser.cs:

    private Option<Expression> ParseLogicalAnd()
    {
        var expr = ParseEquality();
        if (expr.IsNone) return None<Expression>();

        var result = expr.Unwrap();

        while (Match(TokenType.And)) // &&
        {
            var op = Previous();
            var rightResult = ParseEquality();
            if (rightResult.IsNone)
            {
                ErrorHandler.Report("Expected expression after operator.", op.Span);
                return Some(result);
            }

            var right = rightResult.Unwrap();
            var span = new Span(result.Span.Start, right.Span.End);
            result = new BinaryExpression(result, op, right, span);
        }

        return Some(result);
    }

    private Option<Expression> ParseEquality()
    {
        var exprResult = ParseComparison();
        if (exprResult.IsNone) return None<Expression>();

        var expr = exprResult.Unwrap();

        while (Match(TokenType.BangEqual, TokenType.EqualEqual))
        {
            var op = Previous();
            var rightResult = ParseComparison();
            if (rightResult.IsNone)
            {
                // Error recovery: return what we have so far
                ErrorHandler.Report("Expected expression after operator.", op.Span);
                return Some(expr);
            }

            var right = rightResult.Unwrap();
            var span = new Span(expr.Span.Start, right.Span.End);
            expr = new BinaryExpression(expr, op, right, span);
        }

        return Some(expr);
    }

    private Option<Expression> ParseComparison()
    {
        var exprResult = ParseTerm();
        if (exprResult.IsNone) return None<Expression>();

        var expr = exprResult.Unwrap();

        while (Match(TokenType.Greater, TokenType.GreaterEqual, TokenType.Less, TokenType.LessEqual))
        {
            var op = Previous();
            var rightResult = ParseTerm();
            if (rightResult.IsNone)
            {
                ErrorHandler.Report("Expected expression after operator.", op.Span);
                return Some(expr);
            }

            var right = rightResult.Unwrap();
            var span = new Span(expr.Span.Start, right.Span.End);
            expr = new BinaryExpression(expr, op, right, span);
        }

        return Some(expr);
    }

    private Option<Expression> ParseTerm()
    {
        var exprResult = ParseFactor();
        if (exprResult.IsNone) return None<Expression>();

        var expr = exprResult.Unwrap();

        while (Match(TokenType.Minus, TokenType.Plus))
        {
            var op = Previous();
            var rightResult = ParseFactor();
            if (rightResult.IsNone)
            {
                ErrorHandler.Report("Expected expression after operator.", op.Span);
                return Some(expr);
            }

            var right = rightResult.Unwrap();
            var span = new Span(expr.Span.Start, right.Span.End);
            expr = new BinaryExpression(expr, op, right, span);
        }

        return Some(expr);
    }

    private Option<Expression> ParseFactor()
    {
        var exprResult = ParseUnary();
        if (exprResult.IsNone) return None<Expression>();

        var expr = exprResult.Unwrap();

        while (Match(TokenType.Slash, TokenType.Star))
        {
            var op = Previous();
            var rightResult = ParseUnary();
            if (rightResult.IsNone)
            {
                ErrorHandler.Report("Expected expression after operator.", op.Span);
                return Some(expr);
            }

            var right = rightResult.Unwrap();
            var span = new Span(expr.Span.Start, right.Span.End);
            expr = new BinaryExpression(expr, op, right, span);
        }

        return Some(expr);
    }

    private Option<Expression> ParseUnary()
    {
        if (Match(TokenType.Bang, TokenType.Minus))
        {
            var op = Previous();
            var rightResult = ParseUnary();
            if (rightResult.IsNone)
            {
                ErrorHandler.Report("Expected expression after unary operator.", op.Span);
                return None<Expression>();
            }

            var right = rightResult.Unwrap();
            var span = new Span(op.Span.Start, right.Span.End);
            return Some<Expression>(new UnaryExpression(op, right, span));
        }

        return ParsePrimary();
    }

    private Option<Expression> ParsePrimary()
    {
        if (Match(TokenType.True))
            return Some<Expression>(new BooleanLiteralExpression(true, Previous().Span));

        if (Match(TokenType.False))
            return Some<Expression>(new BooleanLiteralExpression(false, Previous().Span));

        if (Match(TokenType.IntegerLiteral))
        {
            var token = Previous();
            var valueText = token.Span.Slice(SourceCode);
            if (int.TryParse(valueText, out var intValue))
            {
                return Some<Expression>(new IntegerLiteralExpression(intValue, token.Span));
            }
            else
            {
                ErrorHandler.Report($"Invalid integer literal: {valueText}", token.Span);
                return None<Expression>();
            }
        }

        if (Match(TokenType.FloatLiteral))
        {
            var token = Previous();
            var valueText = token.Span.Slice(SourceCode);
            if (double.TryParse(valueText, out var floatValue))
            {
                return Some<Expression>(new FloatLiteralExpression(floatValue, token.Span));
            }
            else
            {
                ErrorHandler.Report($"Invalid float literal: {valueText}", token.Span);
                return None<Expression>();
            }
        }

        if (Match(TokenType.StringLiteral))
        {
            var token = Previous();
            var value = token.Span.Slice(SourceCode);
            // Remove quotes
            var stringValue = value[1..^1];
            return Some<Expression>(new StringLiteralExpression(stringValue, token.Span));
        }

        if (Match(TokenType.Identifier))
        {
            var token = Previous();
            var name = token.Span.Slice(SourceCode);

            // Check if this is a function call
            if (Match(TokenType.LeftParen))
            {
                var arguments = new List<Expression>();

                if (!Check(TokenType.RightParen))
                {
                    do
                    {
                        var argResult = ParseExpression();
                        if (argResult.IsNone) return None<Expression>();
                        arguments.Add(argResult.Unwrap());
                    } while (Match(TokenType.Comma));
                }

                var rightParen = TryConsume(TokenType.RightParen, "Expected ')' after arguments.");
                var endPos = rightParen.Match(some => some.Span.End, () => token.Span.End);
                var span = new Span(token.Span.Start, endPos);

                return Some<Expression>(new CallExpression(new IdentifierExpression(name, token.Span), arguments, span));
            }

            // Regular identifier
            return Some<Expression>(new IdentifierExpression(name, token.Span));
        }

        if (Match(TokenType.LeftParen))
        {
            var exprResult = ParseExpression();
            if (exprResult.IsNone) return None<Expression>();

            var expr = exprResult.Unwrap();
            var rightParen = TryConsume(TokenType.RightParen, "Expected ')' after expression.");

            return Some<Expression>(new GroupingExpression(expr, expr.Span));
        }

        ErrorHandler.Report("Expected expression.", Peek().Span);
        return None<Expression>();
    }
}
