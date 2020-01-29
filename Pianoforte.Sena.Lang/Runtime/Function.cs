using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Linq;
using System.Linq.Expressions;
using System.Collections;

namespace Pianoforte.Sena.Lang.Runtime
{
  public enum FunctionKind
  {
    Lambda,
    Func,
  }

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

    public Value this[int i] => values[i];

    public IEnumerator<Value> GetEnumerator()
      => values.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
      => values.GetEnumerator();
  }

  public class Function
  {
    public FunctionKind Kind
    {
      get => Lambda != null ? FunctionKind.Lambda : FunctionKind.Func;
    }

    public LambdaExpression Lambda { get; }
    public Func<FunctionArgs, Value> Func { get; }

    Delegate compiledLamda = null;

    public string Name { get; }
    public string[] ArgNames { get; }
    public int ArgsCount => ArgNames.Length;

    public Function(LambdaExpression lambda)
    {
      Lambda = lambda;
      Name = Lambda.Name;
      ArgNames = Lambda.Parameters.Select((p) => p.Name).ToArray();
    }

    public Function(Func<FunctionArgs, Value> func, string name, params string[] argNames)
    {
      Func = func;
      Name = name;
      ArgNames = argNames;
    }

    public override string ToString()
    {
      var argsStr = string.Join(", ", ArgNames);
      return string.Format("func {0}({1})", Name, argsStr);
    }

    public Delegate Compile()
    {
      return compiledLamda = Lambda.Compile();
    }

    private Value CallLambda(FunctionArgs args)
    {
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

      return obj switch
      {
        Value v => v,
        _ =>
          throw new InternalAssertionException(
            string.Format("Return value must be Runtime.Value not {0}", obj.GetType())
          ),
      };
    }
    private Value CallFunc(FunctionArgs args)
      => Func.Invoke(args);

    public Value Call(params Value[] args)
      => Call(new FunctionArgs(args));

    public Value Call(FunctionArgs args)
    {
      if (args.Count != ArgsCount)
      {
        throw new RuntimeException(
          string.Format(
            Properties.Resources.InvalidNumberOfArgs,
            ArgsCount,
            args.Count
          )
        );
      }
      return Kind == FunctionKind.Lambda ? CallLambda(args) : CallFunc(args);
    }
  }
}
