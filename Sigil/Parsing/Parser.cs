﻿using Sigil.Common;
using Sigil.ErrorHandling;
using Sigil.Lexing;
using Sigil.Parsing.Expressions;
using Sigil.Parsing.Statements;

namespace Sigil.Parsing;

public class Parser(List<Token> Tokens, ErrorHandler ErrorHandler, string SourceCode)
{
    private int _current = 0;
    private Stack<ExecutionContext> _contextStack = new([ExecutionContext.TopLevel]);

    private Token Peek() => _current >= Tokens.Count ? Tokens.Last() : Tokens[_current];
    private Token Previous() => Tokens[_current - 1];
    private bool IsAtEnd() => Peek().TokenType == TokenType.Eof;
    private bool IsInFunction => _contextStack.Contains(ExecutionContext.Function);

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

    private Option<Statement> ParseReturnStatement()
    {
        var start = Previous().Span.Start;

        Expression? expression = null;
        if (!Check(TokenType.Semicolon))
        {
            var exprResult = ParseExpression();
            if (exprResult.IsNone) return None<Statement>(); // Error parsing expression
            expression = exprResult.Unwrap();
        }

        var semicolon = TryConsume(TokenType.Semicolon, "Expected ';' after return value.");

        var endPos = semicolon.Match(
            some => some.Span.End,
            () => expression?.Span.End ?? start
        );

        var span = new Span(start, endPos);
        return Some<Statement>(new ReturnStatement(expression, span));
    }

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

        _contextStack.Push(ExecutionContext.Function);

        var body = new List<Statement>();

        try
        {
            while (!Check(TokenType.RightBrace) && !IsAtEnd())
            {
                var stmt = ParseStatement();
                stmt.EffectSome(body.Add);
            }

        }
        finally
        {
            _contextStack.Pop();
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
        return ParseBinary(expr.Unwrap(), ParseLogicalAnd, TokenType.Or);
    }

    private Option<Expression> ParseLogicalAnd()
    {
        var expr = ParseEquality();
        if (expr.IsNone) return None<Expression>();
        return ParseBinary(expr.Unwrap(), ParseEquality, TokenType.And);
    }

    private Option<Expression> ParseEquality()
    {
        var expr = ParseComparison();
        if (expr.IsNone) return None<Expression>();
        return ParseBinary(expr.Unwrap(), ParseComparison, TokenType.BangEqual, TokenType.EqualEqual);
    }

    private Option<Expression> ParseComparison()
    {
        var expr = ParseTerm();
        if (expr.IsNone) return None<Expression>();
        return ParseBinary(expr.Unwrap(), ParseTerm,
            TokenType.Greater, TokenType.GreaterEqual, TokenType.Less, TokenType.LessEqual);
    }

    private Option<Expression> ParseTerm()
    {
        var expr = ParseFactor();
        if (expr.IsNone) return None<Expression>();
        return ParseBinary(expr.Unwrap(), ParseFactor, TokenType.Minus, TokenType.Plus);
    }

    private Option<Expression> ParseFactor()
    {
        var expr = ParseUnary();
        if (expr.IsNone) return None<Expression>();
        return ParseBinary(expr.Unwrap(), ParseUnary, TokenType.Slash, TokenType.Star);
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

        return ParseCall();
    }

    private Option<Expression> ParseCall()
    {
        var expr = ParsePrimary();
        if (expr.IsNone) return None<Expression>();

        while (true)
        {
            if (Match(TokenType.LeftParen))
            {
                expr = FinishCall(expr.Unwrap());
                if (expr.IsNone) return None<Expression>();
            }
            else
            {
                break;
            }
        }

        return expr;
    }
    private Option<Expression> FinishCall(Expression callee)
    {
        var arguments = new List<Expression>();
        var start = Previous().Span.Start;

        if (!Check(TokenType.RightParen))
        {
            do
            {
                if (arguments.Count >= 255)
                {
                    ErrorHandler.Report("Cannot have more than 255 arguments.", Peek().Span);
                }

                var arg = ParseExpression();
                if (arg.IsNone) return None<Expression>();
                arguments.Add(arg.Unwrap());
            } while (Match(TokenType.Comma));
        }

        var paren = TryConsume(TokenType.RightParen, "Expected ')' after arguments.");
        var endPos = paren.Match(some => some.Span.End, () => start);
        var span = new Span(callee.Span.Start, endPos);

        return Some<Expression>(new CallExpression(callee, arguments, span));
    }

    private Option<Expression> ParseBinary(Expression left, Func<Option<Expression>> operandParser, params TokenType[] operators)
    {
        var expr = left;

        while (Match(operators))
        {
            var op = Previous();
            var rightResult = operandParser();
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

        if (Match(TokenType.CharacterLiteral))
        {
            var token = Previous();
            var valueText = token.Span.Slice(SourceCode);
            var innerText = valueText[1..^1];

            char finalChar;
            if (innerText.StartsWith('\\'))
            {
                // The lexer should ensure this is a valid 2-character escape sequence
                if (innerText.Length != 2)
                {
                    ErrorHandler.Report($"Malformed escape sequence in char literal: {valueText}", token.Span);
                    return None<Expression>();
                }
                finalChar = innerText[1] switch
                {
                    '0' => '\0',
                    'n' => '\n',
                    'r' => '\r',
                    't' => '\t',
                    '\\' => '\\',
                    '\'' => '\'',
                    '"' => '"',
                    _ => ' ' // Should be caught by lexer
                };
            }
            else
            {
                // The lexer should ensure this is a single character
                finalChar = innerText[0];
            }
            return Some<Expression>(new CharacterLiteralExpression(finalChar, token.Span));
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
