using System;
using System.Collections.Generic;
using System.Text;

namespace Pianoforte.Sena.Lang
{
  public enum RuntimeValueType
  {
    None,
    Bool,
    Number,
    String,
  }

  public struct RuntimeValue
  {
    public const string NoneStr = "None";
    public const string TrueStr = "True";
    public const string FalseStr = "False";
    public RuntimeValueType Type { get; set; }

    private void AssertType(RuntimeValueType t)
    {
      if (Type != t)
      {
        throw new Exception("Type mismatch");
      }
    }

    #region Properties

    private bool _bool;
    public bool Bool
    {
      get
      {
        AssertType(RuntimeValueType.Bool);
        return _bool;
      }
      set { _bool = value; }
    }

    private decimal _number;
    public decimal Number
    {
      get
      {
        AssertType(RuntimeValueType.Number);
        return _number;
      }
      set { _number = value; }
    }

    private string  _string;
    public string String
    {
      get
      {
        AssertType(RuntimeValueType.String);
        return _string;
      }
      set { _string = value; }
    }

    #endregion

    #region Constructors
    public RuntimeValue(RuntimeValueType type) : this(type, false, 0, "")
    {
    }

    public RuntimeValue(RuntimeValueType type, bool b, decimal num, string str)
    {
      Type = type;
      _bool = b;
      _number = num;
      _string = str;
    }
    #endregion

    #region FromXXX

    public static RuntimeValue FromObject(object value)
    {
      switch (value)
      {
        case bool b:
          return MakeBool(b);
        case decimal n:
          return MakeNumber(n);
        case string s:
          return MakeString(s);
        case null:
          return MakeNone();
      }
      throw new Exception("Cannot convert RuntimeValue");
    }

    public static RuntimeValue FromToken(Token token)
    {
      RuntimeValueType type;
      switch(token.Kind)
      {
        case TokenKind.NoneLiteral:
          type = RuntimeValueType.None;
          break;
        case TokenKind.TrueLiteral:
          type = RuntimeValueType.Bool;
          break;
        case TokenKind.FalseLiteral:
          type = RuntimeValueType.Bool;
          break;
        case TokenKind.NumberLiteral:
          type = RuntimeValueType.Number;
          break;
        case TokenKind.StringLiteral:
          type = RuntimeValueType.String;
          break;
        default:
          throw new Exception("Cannot convert RuntimeValue");
      }
      return FromString(type, token.Text);
    }

    public static RuntimeValue FromString(RuntimeValueType type, string v)
    {
      if (type == RuntimeValueType.None && v == NoneStr)
      {
        return MakeNone();
      }
      else if (type == RuntimeValueType.Bool && (v == TrueStr || v == FalseStr))
      {
        return MakeBool(v == TrueStr);
      }
      else if (type == RuntimeValueType.Number)
      {
        decimal n;
        decimal.TryParse(v, out n);
        return MakeNumber(n);
      }
      else if (type == RuntimeValueType.String)
      {
        return MakeString(v);
      }
      throw new Exception(string.Format("{0} is not {1}", v, type)); 
    }
    #endregion

    #region MakeXXX
    public static RuntimeValue MakeNone()
    {
      return new RuntimeValue(RuntimeValueType.None);
    }

    public static RuntimeValue MakeBool(bool v)
    {
      return new RuntimeValue(RuntimeValueType.Bool, v, 0, "");
    }

    public static RuntimeValue MakeNumber(decimal v)
    {
      return new RuntimeValue(RuntimeValueType.Number, false, v, "");
    }

    public static RuntimeValue MakeString(string v)
    {
      return new RuntimeValue(RuntimeValueType.String, false, 0, v);
    }
    #endregion

    #region Converts

    public override string ToString()
    {
      switch (Type)
      {
        case RuntimeValueType.None:
          return NoneStr;
        case RuntimeValueType.Bool:
          return Bool ? TrueStr : FalseStr;
        case RuntimeValueType.Number:
          return Number.ToString();
        case RuntimeValueType.String:
          return String;
      }
      throw new Exception("Unknown RuntimeValueType");
    }

    public RuntimeValue ConvertType(RuntimeValueType type)
    {
      return RuntimeValue.FromString(type, ToString());
    }

    #endregion
  }
}
