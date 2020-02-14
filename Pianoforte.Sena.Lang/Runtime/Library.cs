using System;
using System.Collections.Generic;
using System.Text;
using System.Linq.Expressions;

namespace Pianoforte.Sena.Lang.Runtime
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

    public IConsole Console { get; }
    public interface IConsole
    {
      void WriteLine(Value v);

      public Object AsObject()
        => new Object("Console", new Dictionary<string, Value>() {
          { "WriteLine", makeVoidFunc((args) => WriteLine(args[0]), "WriteLine", "v") },
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
