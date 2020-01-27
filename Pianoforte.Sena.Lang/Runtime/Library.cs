using System;
using System.Collections.Generic;
using System.Text;
using System.Linq.Expressions;

namespace Pianoforte.Sena.Lang.Runtime
{
  public class Library
  {
    public interface IConsole
    {
      void WriteLine(Value v);

      private Function WriteLineFunction
      {
        get => new Function((args) =>
        {
          WriteLine(args[0]);
          return Value.MakeNone();
        }, "WriteLine", "v");
      }
      public Object AsObject()
      {
        return new Object("Console", new Dictionary<string, Value>() {
          { "WriteLine", Value.MakeFunction(WriteLineFunction) },
        });
      }
    }
    public IConsole Console { get; }

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
