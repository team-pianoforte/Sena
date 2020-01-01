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
    private Token lookahead;

    public Parser(Lexer lexer)
    {
      this.lexer = lexer;
      ReadToken();
    }

    private Token ReadToken()
    {
      var tok = lookahead;
      lookahead = lexer.Next();
      return tok;
    }

    private Expression ParseFactor()
    {
      var tok = ReadToken();
      switch (tok.Kind)
      {
        case TokenKind.NumberLiteral:
        case TokenKind.StringLiteral:
          return Expression.Constant(Runtime.Value.FromToken(tok));
      }
      throw new Exception("Invalid token");
    }

    private Expression ParseExpr()
    {
      switch (lookahead.Kind)
      {
        case TokenKind.NumberLiteral:
        case TokenKind.StringLiteral:
          return ParseFactor();
      }
      throw new Exception(string.Format("Unexpected {0}", lookahead.Kind));
    }

    private Expression ParseLine()
    {
      var expr = ParseExpr();
      var eol = ReadToken();
      if (!(eol.Kind == TokenKind.EndOfFile || eol.Kind == TokenKind.EndOfLine))
      {
        throw new Exception("Syntax Error");
      }
      return expr;
    }

    public LambdaExpression Parse()
    {

      var lines = new List<Expression>();
      while (lookahead.Kind != TokenKind.EndOfFile)
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
