using System;
using System.Collections.Generic;
using System.Text;
using System.Linq.Expressions;

namespace Pianoforte.Sena.Lang.Runtime
{
  public class Environment
  {
    public Object Console { get; }
    public Block RootBlock
    {
      get
      {
        return new Block(null, new Dictionary<string, Value>()
        {
          { "Console", Value.MakeObject(Console) },
        });
      }
    }

    public Environment(Object console)
    {
      Console = console;
    }

    private static Function cliWriteLine
    {
      get
      {
        var v = Expression.Parameter(typeof(Value), "v");
        return new Function(
          Expression.Lambda(
            Expression.Call(
              typeof(Console).GetMethod("WriteLine", new Type[] { typeof(string) }),
              Expression.Call(v, typeof(Runtime.Value).GetMethod("ToString"))
            ),
            "WriteLine",
            new[] { v }
          )
        );
      }
    }

    public static Environment Cli = new Environment(new Object("Console", new Dictionary<string, Value>()
    {
      { "WriteLine", Value.MakeFunction(cliWriteLine) },
    }));
  }
}
