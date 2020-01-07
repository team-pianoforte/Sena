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

    private Expression ParseValue()
    {
      var tok = NextToken();
      var v = tok switch
      {
        var c when c.IsLiteral() => Syntax.Literal(tok),
        var c when c.Kind == TokenKind.Identifier => Syntax.Variable(tok.Text),
        _ => throw new SyntaxException(tok, string.Format(Properties.Resources.UnexpectedToken, tok.Kind)),
      };

      while (lookahead[0].Kind == TokenKind.Dot)
      {
        NextToken();
        var nameTok = NextToken();
        v = Syntax.MemberAccess(v, nameTok.Text);
      }
      return v;
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

        // TODO: It is debugging
        expr = Expression.Call(
          typeof(Console).GetMethod("WriteLine", new Type[] { typeof(string) }),
          Expression.Call(expr, typeof(Runtime.Value).GetMethod("ToString"))
        );
      }
      var eol = NextToken();
      if (!(eol.Kind == TokenKind.EndOfFile || eol.Kind == TokenKind.EndOfLine))
      {
        throw new SyntaxException(eol, string.Format(Properties.Resources.InvalidToken, TokenKind.EndOfLine, eol.Kind));
      }
      return expr;
    }

    public LambdaExpression Parse()
    {

      var lines = new List<Expression>();
      while (lookahead[0].Kind != TokenKind.EndOfFile)
      {
        lines.Add(ParseLine());
      }


      return Expression.Lambda(Syntax.Block(null, lines));
    }
  }
}
