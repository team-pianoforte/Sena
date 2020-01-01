using System;
using System.Collections.Generic;
using System.Text;

namespace Pianoforte.Sena.Lang.Runtime
{
  public enum ValueType
  {
    None,
    Bool,
    Number,
    String,
  }

  public struct Value
  {
    public const string NoneStr = "None";
    public const string TrueStr = "True";
    public const string FalseStr = "False";
    public ValueType Type { get; set; }

    private void AssertType(ValueType t)
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
        AssertType(ValueType.Bool);
        return _bool;
      }
      set { _bool = value; }
    }

    private decimal _number;
    public decimal Number
    {
      get
      {
        AssertType(ValueType.Number);
        return _number;
      }
      set { _number = value; }
    }

    private string  _string;
    public string String
    {
      get
      {
        AssertType(ValueType.String);
        return _string;
      }
      set { _string = value; }
    }

    #endregion

    #region Constructors
    public Value(ValueType type) : this(type, false, 0, "")
    {
    }

    public Value(ValueType type, bool b, decimal num, string str)
    {
      Type = type;
      _bool = b;
      _number = num;
      _string = str;
    }
    #endregion

    #region FromXXX

    public static Value FromObject(object value)
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

    public static Value FromToken(Token token)
    {
      ValueType type;
      switch(token.Kind)
      {
        case TokenKind.NoneLiteral:
          type = ValueType.None;
          break;
        case TokenKind.TrueLiteral:
          type = ValueType.Bool;
          break;
        case TokenKind.FalseLiteral:
          type = ValueType.Bool;
          break;
        case TokenKind.NumberLiteral:
          type = ValueType.Number;
          break;
        case TokenKind.StringLiteral:
          type = ValueType.String;
          break;
        default:
          throw new Exception("Cannot convert RuntimeValue");
      }
      return FromString(type, token.Text);
    }

    public static Value FromString(ValueType type, string v)
    {
      if (type == ValueType.None && v == NoneStr)
      {
        return MakeNone();
      }
      else if (type == ValueType.Bool && (v == TrueStr || v == FalseStr))
      {
        return MakeBool(v == TrueStr);
      }
      else if (type == ValueType.Number)
      {
        decimal n;
        decimal.TryParse(v, out n);
        return MakeNumber(n);
      }
      else if (type == ValueType.String)
      {
        return MakeString(v);
      }
      throw new Exception(string.Format("{0} is not {1}", v, type)); 
    }
    #endregion

    #region MakeXXX
    public static Value MakeNone()
    {
      return new Value(ValueType.None);
    }

    public static Value MakeBool(bool v)
    {
      return new Value(ValueType.Bool, v, 0, "");
    }

    public static Value MakeNumber(decimal v)
    {
      return new Value(ValueType.Number, false, v, "");
    }

    public static Value MakeString(string v)
    {
      return new Value(ValueType.String, false, 0, v);
    }
    #endregion

    #region Converts

    public override string ToString()
    {
      switch (Type)
      {
        case ValueType.None:
          return NoneStr;
        case ValueType.Bool:
          return Bool ? TrueStr : FalseStr;
        case ValueType.Number:
          return Number.ToString();
        case ValueType.String:
          return String;
      }
      throw new Exception("Unknown RuntimeValueType");
    }

    public Value ConvertType(ValueType type)
    {
      return Value.FromString(type, ToString());
    }

    #endregion

  }
}
