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

    private SyntaxTree.AST ParseArray()
    {
      if (NextToken().Kind != TokenKind.SquareBracketLeft)
      {
        throw new InternalAssertionException("Invalid token");
      }

      if (lookahead.Head.Kind == TokenKind.ParenRight)
      {
        return new SyntaxTree.InitArray(lookahead.Head);
      }

      var items = new List<SyntaxTree.AST>();
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
      return new SyntaxTree.InitArray(lookahead.Head, items);
    }

    private SyntaxTree.AST ParseLiteral()
    {
      var tok = NextToken();
      return new SyntaxTree.Literal(tok);
    }
    private SyntaxTree.AST ParseVariable()
    {
      var tok = NextToken();
      return new SyntaxTree.Variable(tok);
    }
    private SyntaxTree.AST ParseValue()
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
            expr = new SyntaxTree.MemberAccess(nameTok, expr, nameTok.Text);
            break;
          case TokenKind.ParenLeft:
            expr = ParseFunctionCall(expr);
            break;
          case TokenKind.SquareBracketLeft:
            expr = new SyntaxTree.ArrayItem(lookahead.Head, expr, ParseArrayIndex());
            break;
          default:
            loop = false;
            break;
        }
      }
      return expr;
    }

    private SyntaxTree.AST ParseFactor()
    {
      var expr = ParseValue();
      while (true)
      {
        if (lookahead.Head.IsFactorOp())
        {
          expr = new SyntaxTree.Binary(NextToken(), expr, ParseValue());
        }
        else
        {
          return expr;
        }
      }
    }

    private SyntaxTree.AST ParseTerm()
    {
      var expr = ParseFactor();
      while (true)
      {
        if (lookahead.Head.IsTermOp())
        {
          expr = new SyntaxTree.Binary(NextToken(), expr, ParseFactor());
        }
        else
        {
          return expr;
        }
      }
    }

    private SyntaxTree.AST ParseExpr()
    {
      return ParseTerm();
    }

    private SyntaxTree.ASTList ParseFunctionArgs()
    {
      var parentLeft = NextToken();
      AssertTokenKind(TokenKind.ParenLeft, parentLeft);
      if (lookahead.Head.Kind == TokenKind.ParenRight)
      {
        return new SyntaxTree.ASTList(lookahead.Head);
      }

      var args = new List<SyntaxTree.AST>() { ParseExpr() };
      while (lookahead.Head.Kind == TokenKind.Comma)
      {
        NextToken();
        args.Add(ParseExpr());
      }
      AssertTokenKind(TokenKind.ParenRight, NextToken());
      return new SyntaxTree.ASTList(parentLeft, args);
    }

    private SyntaxTree.AST ParseFunctionCall(SyntaxTree.AST func)
    {
      return new SyntaxTree.FunctionCall(lookahead.Head, func, ParseFunctionArgs());
    }

    private SyntaxTree.AST ParseArrayIndex()
    {
      var tok = NextToken();
      AssertTokenKind(TokenKind.SquareBracketLeft, tok);

      var v = ParseExpr();

      tok = NextToken();
      AssertTokenKind(TokenKind.SquareBracketRight, tok);
      return v;
    }

    private IEnumerable<SyntaxTree.AST> ParseUntilTokenKind(TokenKind kind)
    {
      while (lookahead[0].Kind != kind)
      {
        yield return ParseStatement();
      }
    }

    private SyntaxTree.AST ParseBeginBlock()
    {
      var begin = NextToken();
      AssertTokenKind(TokenKind.Begin, begin);
      AssertTokenKind(TokenKind.EndOfLine, NextToken());

      var ast = new SyntaxTree.Block(begin, ParseUntilTokenKind(TokenKind.End));
      AssertTokenKind(TokenKind.End, NextToken());

      return ast;
    }

    private SyntaxTree.AST ParseAssignment()
    {
      var ident = lookahead[0];
      if (ident.Kind != TokenKind.Identifier)
      {
        throw new SyntaxException(ident, Properties.Resources.InvalidLeftOfAssignment);
      }
      NextToken();
      NextToken();
      return new SyntaxTree.Assign(ident, ParseExpr());
    }

    private SyntaxTree.AST ParseStatement()
    {
      SyntaxTree.AST expr = lookahead[0].Kind switch
      {
        TokenKind.Begin => ParseBeginBlock(),
        _ => lookahead[1].Kind == TokenKind.OpAssignment ? ParseAssignment() : ParseExpr(),
      };
      var eol = NextToken();
      if (!(eol.Kind == TokenKind.EndOfFile || eol.Kind == TokenKind.EndOfLine))
      {
        throw new SyntaxException(eol, string.Format(Properties.Resources.InvalidToken, TokenKind.EndOfLine, eol.Kind));
      }
      return expr;
    }

    public SyntaxTree.RootBlock Parse(Runtime.Environment env)
      => new SyntaxTree.RootBlock(new Token(), env.RootBlock, ParseUntilTokenKind(TokenKind.EndOfFile));
  }
}
