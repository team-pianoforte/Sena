using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Pianoforte.Sena.Lang
{
  public class Lexer : IDisposable
  {
    private readonly StreamReader inputReader;
    private char lookahead;
    private TokenPosition position;
    private bool endOfInput;

    private bool LookaheadIsEol { get => !endOfInput && lookahead == '\n'; }

    public Lexer(string filename, Stream input)
    {
      inputReader = new StreamReader(input);
      position = new TokenPosition(filename, 1, 1);
      Consume();
    }


    #region IDisposable Support
    private bool disposed = false;

    protected virtual void Dispose(bool disposing)
    {
      if (!disposed)
      {
        if (disposing)
        {
          inputReader.Dispose();
        }
        disposed = true;
      }
    }

    public void Dispose()
    {
      Dispose(true);
    }
    #endregion

    private void Consume()
    {
      var n = inputReader.Read();
      position.Column += 1;
      if (n == -1)
      {
        lookahead = '\0';
        endOfInput = true;
      }
      else
      {
        lookahead = (char)n;
      }
      if (LookaheadIsEol)
      {
        position.Line += 1;
        position.Column = 1;
      }
    }

    private bool IsWhiteSpace(char c)
    {
      if (LookaheadIsEol)
      {
        return false;
      }
      if (c == '\n')
      {
        return false;
      }
      return char.IsWhiteSpace(c);
    }

    private void SkipWhiteSpaces()
    {
      while (IsWhiteSpace(lookahead))
      {
        Consume();
      }
    }

    private Token ReadNumberLiteral()
    {
      var foundPoint = false;
      var sb = new StringBuilder();
      var pos = position;
      while (char.IsDigit(lookahead) || !foundPoint && lookahead == '.')
      {
        sb.Append(lookahead);
        if (lookahead == '.')
        {
          foundPoint = true;
        }
        Consume();
      }
      return new Token(TokenKind.NumberLiteral, sb.ToString(), pos);
    }

    private Token ReadStringLiteral(char quote)
    {
      var pos = position;

      if (lookahead != quote)
      {
        throw new Exception(String.Format("Unexpected {0}", lookahead));
      }
      Consume();

      var sb = new StringBuilder();
      var escaping = false;
      while (true)
      {
        if (endOfInput || LookaheadIsEol)
        {
          throw new Exception("Unterminated String Literal");
        }

        if (lookahead == '\\')
        {
          if (escaping)
          {
            sb.Append('\\');
          }
          escaping = true;
        }
        if (lookahead == quote)
        {
          if (escaping)
          {
            sb.Append(lookahead);
          }
          else
          {
            Consume();
            return new Token(TokenKind.StringLiteral, sb.ToString(), pos);
          }
        }
        else
        {
          sb.Append(lookahead);
        }
        Consume();
      }
    }

    public Token Next()
    {
      var sb = new StringBuilder();
      SkipWhiteSpaces();

      var pos = position;

      if (LookaheadIsEol)
      {
        Consume();
        return new Token(TokenKind.EndOfLine, "\n", pos);
      }

      if (endOfInput)
      {
        return new Token(TokenKind.EndOfFile, "", pos);
      }

      SkipWhiteSpaces();
      switch (lookahead)
      {
        case char c when char.IsDigit(c):
          return ReadNumberLiteral();
        case '"':
          return ReadStringLiteral('"');
        case '\'':
          return ReadStringLiteral('\'');
          
      }
      throw new Exception("Syntax Error");
    }
  }
}
