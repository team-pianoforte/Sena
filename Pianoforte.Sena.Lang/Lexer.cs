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

      while (!(endOfInput || LookaheadIsEol))
      {
        sb.Append(lookahead);
        Consume();
      }
      return new Token(TokenKind.Literal, sb.ToString(), pos);
    }
  }
}
