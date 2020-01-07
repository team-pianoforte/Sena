﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Linq;
using System.Linq.Expressions;

namespace Pianoforte.Sena.Lang.Runtime
{
  public enum ValueType
  {
    None,
    Bool,
    Number,
    String,
    Object,
    Function,
  }

  public readonly struct Value
  {
    public const string NoneStr = "None";
    public const string TrueStr = "True";
    public const string FalseStr = "False";
    public ValueType Type { get; }

    private void AssertType(ValueType t)
    {
      if (Type != t)
      {
        throw new InternalAssertionException("Type mismatch");
      }
    }

    #region Properties

    private readonly bool _bool;
    public bool Bool
    {
      get
      {
        AssertType(ValueType.Bool);
        return _bool;
      }
    }

    private readonly decimal _number;
    public decimal Number
    {
      get
      {
        AssertType(ValueType.Number);
        return _number;
      }
    }

    private readonly string _string;
    public string String
    {
      get
      {
        AssertType(ValueType.String);
        return _string;
      }
    }

    private readonly Object _object;
    public Object Object
    {
      get
      {
        AssertType(ValueType.Object);
        return _object;
      }
    }

    private readonly Function _function;
    public Function Function
    {
      get
      {
        AssertType(ValueType.Function);
        return _function;
      }
    }

    #endregion

    #region Constructors
    public Value(ValueType type) : this(type, false, 0, "", null, null)
    {
    }

    public Value(ValueType type, bool b, decimal num, string str, Object obj, Function func)
    {
      Type = type;
      _bool = b;
      _number = num;
      _string = str;
      _object = obj;
      _function = func;
    }
    #endregion

    #region FromXXX

    public static Value FromToken(Token token)
    {
      var type = token.Kind switch
      {
        TokenKind.NoneLiteral => ValueType.None,
        TokenKind.TrueLiteral => ValueType.Bool,
        TokenKind.FalseLiteral => ValueType.Bool,
        TokenKind.NumberLiteral => ValueType.Number,
        TokenKind.StringLiteral => ValueType.String,
        _ => throw new InternalAssertionException("Cannot convert RuntimeValue"),
      };
      return MakeString(token.Text).ConvertType(type);
    }

    #endregion

    #region MakeXXX
    public static Value MakeNone()
    {
      return new Value(ValueType.None);
    }

    public static Value MakeBool(bool v)
    {
      return new Value(ValueType.Bool, v, 0, "", null, null);
    }

    public static Value MakeNumber(decimal v)
    {
      return new Value(ValueType.Number, false, v, "", null, null);
    }

    public static Value MakeString(string v)
    {
      return new Value(ValueType.String, false, 0, v, null, null);
    }

    public static Value MakeObject(Object v)
    {
      return new Value(ValueType.Object, false, 0, "", v, null);
    }
    public static Value MakeFunction(Function v)
    {
      return new Value(ValueType.Function, false, 0, "", null, v);
    }
    #endregion

    #region Converts

    public override string ToString() => Type switch
    {
      ValueType.None => NoneStr,
      ValueType.Bool => Bool ? TrueStr : FalseStr,
      ValueType.Number => Number.ToString(),
      ValueType.String => String,
      ValueType.Object => Object.ToString(),
      ValueType.Function => Function.ToString(),
    };

    public Value ConvertType(ValueType type)
    {
      if (Type == type)
      {
        return this;
      }
      var s = ToString();
      if (type == ValueType.None)
      {
        return MakeNone();
      }
      else if (type == ValueType.Bool && (s == TrueStr || s == FalseStr))
      {
        return MakeBool(s == TrueStr);
      }
      else if (type == ValueType.Number)
      {
        if (decimal.TryParse(s, out var n))
        {
          return MakeNumber(n);
        }
      }
      else if (type == ValueType.String)
      {
        return MakeString(s);
      }
      throw new RuntimeException(string.Format(Properties.Resources.CannotConvertType, s, type));
    }

    #endregion

  }
}
