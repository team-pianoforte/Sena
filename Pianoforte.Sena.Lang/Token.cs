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
    Identifier,

    OpAssignment,
    OpPlusAssignment,
    OpMinusAssignment,
    OpMultiplicationAssignment,
    OpDivisionAssignment,
    OpConcatenationAssigment,
    OpEqual,
    OpNotEqual,
    OpLessThan,
    OpLessThanOrEqual,
    OpGreaterThan,
    OpGreaterThanOrEqual,
    OpPlus,
    OpMinus,
    OpMultiplication,
    OpDivision,
    OpConcatenation,
    ParenLeft,
    ParenRight,
    SquareBracketLeft,
    SquareBracketRight,
  }

  public readonly struct Keyword
  {
    public readonly string Text;
    public readonly TokenKind TokenKind;

    public Keyword(string text, TokenKind tokenKind)
    {
      Text = text;
      TokenKind = tokenKind;
    }
  }

  public static class Keywords
  {
    public static readonly Keyword None = new Keyword("none", TokenKind.NoneLiteral);
    public static readonly Keyword True = new Keyword("true", TokenKind.TrueLiteral);
    public static readonly Keyword False = new Keyword("false", TokenKind.TrueLiteral);

    public static readonly Dictionary<string, Keyword> Map = new Dictionary<string, Keyword> {
      { None.Text, None },
      { True.Text, True },
      { False.Text, False },
    };
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

    public override string ToString()
    {
      return string.Format("{0}: {1},{2}", InputName, Line, Column);
    }
  }

  public struct Token
  {
    public TokenKind Kind;
    public string Text;
    public TokenPosition Position;

    public Token(TokenKind kind, string text, TokenPosition pos)
    {
      Kind = kind;
      Text = text;
      Position = pos;
    }

    public override string ToString()
    {
      return string.Format("{0}: {1} {2}", Position.ToString(), Kind.ToString(), Text);
    }
  }
}
