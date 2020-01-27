﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Linq;
using System.Linq.Expressions;
using System.Collections;

namespace Pianoforte.Sena.Lang.Runtime
{

  public class FunctionArgs : IEnumerable<Value>
  {
    private readonly List<Value> values;

    public FunctionArgs(IEnumerable<Value> v)
    {
      values = new List<Value>(v);
    }

    public List<Value> ToList() => values;
    public Value[] ToArray() => values.ToArray();
    public int Count => values.Count;

    public IEnumerator<Value> GetEnumerator()
      => values.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
      => values.GetEnumerator();
  }

  public class Function
  {
    public string Name { get => Lambda.Name; }
    public ReadOnlyCollection<ParameterExpression> Args
    {
      get => Lambda.Parameters;
    }
    public LambdaExpression Lambda { get; }

    Delegate compiledLamda = null;

    public Function(LambdaExpression lambda)
    {
      Lambda = lambda;
    }

    public override string ToString()
    {
      var argsStr = string.Join(", ", Args.Select((v) => v.Name));
      return string.Format("func {0}({1})", Name, argsStr);
    }

    public Delegate Compile()
    {
      return compiledLamda = Lambda.Compile();
    }

    public Value Call(params Value[] args)
      => Call(new FunctionArgs(args));

    public Value Call(FunctionArgs args)
    {
      if (args.Count != Lambda.Parameters.Count)
      {
        throw new RuntimeException(
          string.Format(
            Properties.Resources.InvalidNumberOfArgs,
            Args.Count,
            args.Count
          )
        );
      }
      if (compiledLamda == null)
      {
        Compile();
      }
      var obj = compiledLamda.DynamicInvoke(
        args.Select<Value, object>((v) => (object)v).ToArray()
      );
      if (obj == null)
      {
        return Value.MakeNone();
      }
      switch (obj)
      {
        case Value v: return v;
        default:
          throw new InternalAssertionException(
   string.Format("Return value must be Runtime.Value not {0}", obj.GetType())
 );
      }
    }
  }
}
