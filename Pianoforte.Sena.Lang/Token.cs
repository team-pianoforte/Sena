using System;
using System.Collections.Generic;
using System.Text;

namespace Pianoforte.Sena.Lang
{
  public enum TokenKind
  {
    Noop,
    NoneLiteral,
    TrueLiteral,
    FalseLiteral,
    StringLiteral,
    NumberLiteral,
    EndOfLine,
    EndOfFile,
    Identifier

  }

  public static class Keywords
  {
    public static string None { get; } = "none";
    public static string True { get; } = "true";
    public static string False { get; } = "false";
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
