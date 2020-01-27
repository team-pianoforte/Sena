using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Pianoforte.Sena.Lang
{
  public class Parser
  {
    private readonly Lexer lexer;
    private readonly int lookaheadCount = 2;
    private readonly LookaheadList<Token> lookahead;

    public Parser(Lexer lexer)
    {
      this.lexer = lexer;
      lookahead = new LookaheadList<Token>(lookaheadCount);
      for (var i = 0; i < lookaheadCount; i++)
      {
        lookahead[i] = lexer.Next();
      }
    }

    private Token NextToken()
    {
      var tok = lookahead[0];
      lookahead[lookaheadCount] = lexer.Next();
      lookahead.Next();
      return tok;
    }

    private void AssertTokenKind(TokenKind k, Token tok)
    {
      if (tok.Kind != k)
      {
        var s = Properties.Resources.InvalidToken;
        throw new SyntaxException(tok, string.Format(s, k, tok));
      }
    }

    private Expression ParseArray()
    {
      if (NextToken().Kind != TokenKind.SquareBracketLeft)
      {
        throw new InternalAssertionException("Invalid token");
      }

      var items = new List<Expression>();
      if (lookahead.Head.Kind == TokenKind.ParenRight)
      {
        return Syntax.InitArray(items);
      }

      while (true)
      {
        items.Add(ParseExpr());

        var tok = NextToken();
        if (tok.Kind == TokenKind.SquareBracketRight)
        {
          break;
        }
        AssertTokenKind(TokenKind.Comma, tok);
      }
      return Syntax.InitArray(items);
    }

    private Expression ParseLiteral()
    {
      var tok = NextToken();
      return Syntax.Literal(tok);
    }
    private Expression ParseVariable()
    {
      var tok = NextToken();
      return Syntax.Variable(tok.Text);
    }
    private Expression ParseValue()
    {
      var expr = lookahead.Head switch
      {
        var c when c.IsLiteral() => ParseLiteral(),
        var c when c.Kind == TokenKind.Identifier => ParseVariable(),
        var c when c.Kind == TokenKind.SquareBracketLeft => ParseArray(),
        _ => throw new SyntaxException(lookahead.Head, string.Format(Properties.Resources.UnexpectedToken, lookahead.Head.Kind)),
      };

      var loop = true;
      while (loop)
      {
        switch (lookahead.Head.Kind)
        {
          case TokenKind.Dot:
            NextToken();
            var nameTok = NextToken();
            expr = Syntax.MemberAccess(expr, nameTok.Text);
            break;
          case TokenKind.ParenLeft:
            expr = ParseFunctionCall(expr);
            break;
          case TokenKind.SquareBracketLeft:
            expr = Syntax.ArrayItem(expr, ParseArrayIndex());
            break;
          default:
            loop = false;
            break;
        }
      }
      return expr;
    }

    private Expression ParseFactor()
    {
      return ParseValue();
    }

    private Expression ParseTerm()
    {
      var exp = ParseFactor();
      while (true)
      {
        if (lookahead.Head.IsBinaryOp())
        {
          exp = Syntax.BinaryExpr(exp, NextToken(), ParseFactor());
        }
        else
        {
          return exp;
        }
      }
    }

    private Expression ParseExpr()
    {
      return ParseTerm();
    }

    private Expression ParseFunctionCall(Expression func)
    {
      if (NextToken().Kind != TokenKind.ParenLeft)
      {
        throw new InternalAssertionException("Invalid token");
      }
      if (lookahead.Head.Kind == TokenKind.ParenRight)
      {
        return Syntax.FunctionCall(func);
      }

      IEnumerable<Expression> args = new[] { ParseExpr() };
      while (lookahead.Head.Kind == TokenKind.Comma)
      {
        NextToken();
        args = args.Append(ParseExpr());
      }
      var rightParen = NextToken();
      AssertTokenKind(TokenKind.ParenRight, rightParen);
      return Syntax.FunctionCall(func, args.ToArray());
    }



    private Expression ParseArrayIndex()
    {
      var tok = NextToken();
      AssertTokenKind(TokenKind.SquareBracketLeft, tok);

      var v = ParseExpr();

      tok = NextToken();
      AssertTokenKind(TokenKind.SquareBracketRight, tok);
      return v;
    }

    private IEnumerable<Expression> ParseUntilTokenKind(TokenKind kind)
    {
      while (lookahead[0].Kind != kind)
      {
        yield return ParseLine();
      }
    }

    private Expression ParseLine()
    {
      Expression expr = null;
      if (lookahead[1].Kind == TokenKind.OpAssignment)
      {
        var ident = lookahead[0];
        if (ident.Kind != TokenKind.Identifier)
        {
          throw new SyntaxException(ident, Properties.Resources.InvalidLeftOfAssignment);
        }
        NextToken();
        NextToken();
        expr = Syntax.Assign(ident.Text, ParseExpr());
      }
      else
      {
        expr = ParseExpr();
      }
      var eol = NextToken();
      if (!(eol.Kind == TokenKind.EndOfFile || eol.Kind == TokenKind.EndOfLine))
      {
        throw new SyntaxException(eol, string.Format(Properties.Resources.InvalidToken, TokenKind.EndOfLine, eol.Kind));
      }
      return expr;
    }

    public LambdaExpression Parse(Runtime.Environment env)
    {
      var lines = ParseUntilTokenKind(TokenKind.EndOfFile);
      return Expression.Lambda(Syntax.Block(env.RootBlock, lines));
    }
  }
}
