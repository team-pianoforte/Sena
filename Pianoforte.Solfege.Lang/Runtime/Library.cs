using System;
using System.Collections.Generic;
using System.Text;
using System.Linq.Expressions;

namespace Pianoforte.Solfege.Lang.Runtime
{
  public class Library
  {
    private static Value makeVoidFunc(Action<FunctionArgs> f, string name, params string[] argNames)
      => Value.MakeFunction(
        new Function((args) =>
        {
          f(args);
          return Value.MakeNone();
        }, name, argNames));

    private static Value MakeFunc(Func<FunctionArgs, Value> f, string name, params string[] argNames)
      => Value.MakeFunction(
        new Function(f, name, argNames));

    public IConsole Console { get; }
    public interface IConsole
    {
      void WriteLine(Value v);
      Value ReadLine();

      public Object AsObject()
        => new Object("Console", new Dictionary<string, Value>() {
          { "WriteLine", makeVoidFunc((args) => WriteLine(args[0]), "WriteLine", "v") },
          { "ReadLine", MakeFunc((args) => ReadLine(), "ReadLine") },
        });
    }
    public interface IDebug
    {
      void Error(Value v);

      public Object AsObject()
        => new Object("Debug", new Dictionary<string, Value>() {
          { "Error", makeVoidFunc((args) => Error(args[0]), "Error", "v") },
        });
    }

    public IEnumerable<Object> Objects
    {
      get => new List<Object> { Console.AsObject() };
    }

    public Library(IConsole console)
    {
      Console = console;
    }
  }
}
