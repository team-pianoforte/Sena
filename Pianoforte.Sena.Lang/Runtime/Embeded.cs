using System;
using System.Collections.Generic;
using System.Text;
using System.Linq.Expressions;

namespace Pianoforte.Sena.Lang.Runtime
{
  public static class Embeded
  {
    public static Object Console = new Object("Console", new Dictionary<string, Value>()
    {
      { "WriteLine", Value.MakeFunction(WriteLine) },
    });

    public static readonly Block RootBlock = new Block(null, new Dictionary<string, Value>()
    {
      { "Console", Value.MakeObject(Console) },
    });

    private static Function WriteLine
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
  }
}
