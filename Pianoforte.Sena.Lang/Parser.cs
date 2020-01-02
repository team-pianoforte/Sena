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
      var tok = lookahead.Next();
      lookahead[lookaheadCount] = lexer.Next();
      return tok;
    }

    private Expression ParseFactor()
    {
      var tok = NextToken();
      switch (tok.Kind)
      {
        case TokenKind.NumberLiteral:
        case TokenKind.StringLiteral:
          return Expression.Constant(Runtime.Value.FromToken(tok));
      }
      throw new Exception("Invalid token");
    }

    private Expression ParseTerm()
    {
      var exp = ParseFactor();
      while(true)
      {
        switch (lookahead[0].Kind)
        {
          case TokenKind.OpPlus:
            NextToken();
            exp = Expression.Call(null, typeof(Runtime.Operations).GetMethod("Add"), exp, ParseFactor());
            break;
          default:
            return exp;
        }
      }
    }

    private Expression ParseExpr()
    {
      switch (lookahead[0].Kind)
      {
        case TokenKind.NumberLiteral:
        case TokenKind.StringLiteral:
          return ParseTerm();
      }
      throw new Exception(string.Format("Unexpected {0}", lookahead[0].Kind));
    }

    private Expression ParseLine()
    {
      var expr = ParseExpr();
      var eol = NextToken();
      if (!(eol.Kind == TokenKind.EndOfFile || eol.Kind == TokenKind.EndOfLine))
      {
        throw new Exception("Syntax Error");
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
      var writeLine = typeof(Console).GetMethod("WriteLine", new Type[] { typeof(string) });


      return Expression.Lambda(
        Expression.Block(
          lines.Select((line) => Expression.Call(writeLine, Expression.Call(line, typeof(Runtime.Value).GetMethod("ToString"))))
        )
      );
    }
  }
}
