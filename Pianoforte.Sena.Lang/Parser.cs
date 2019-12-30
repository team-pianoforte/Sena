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

    private Expression ParseLine()
    {
      var tok = ReadToken();
      var eol = ReadToken();
      if (!(eol.Kind == TokenKind.EndOfFile || eol.Kind == TokenKind.EndOfLine))
      {
        throw new Exception("Syntax Error");
      }
      return Expression.Constant(tok.Text);
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
          lines.Select((line) => Expression.Call(writeLine, line))
        )
      );
    }
  }
}
