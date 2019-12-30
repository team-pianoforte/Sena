using System;
using System.Collections.Generic;
using System.Text;

namespace Pianoforte.Sena.Lang
{
  public enum TokenKind
  {
    Noop,
    StringLiteral,
    EndOfLine,
    EndOfFile,
  }

  public struct TokenPosition
  {
    public string InputName;
    public int Line, Column;

    public TokenPosition(string inputName, int line, int column)
    {
      InputName = inputName;
      Line = line;
      Column = column;
    }
  }

  public struct Token
  {
    public TokenKind Kind;
    public string Text;
    public TokenPosition Pos;

    public Token(TokenKind kind, string text, TokenPosition pos)
    {
      Kind = kind;
      Text = text;
      Pos = pos;
    }
  }
}
