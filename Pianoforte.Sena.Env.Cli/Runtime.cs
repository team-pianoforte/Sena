using System;
using System.Collections.Generic;
using System.Text;
using System.Linq.Expressions;


namespace Pianoforte.Sena.Lang.Runtime
{
  internal static class CliRuntime
  {
    private static Function writeLine
    {
      get
      {
        var v = Expression.Parameter(typeof(Value), "v");
        return new Function(
          Expression.Lambda(
            Expression.Call(
              typeof(Console).GetMethod("WriteLine", new Type[] { typeof(string) }),
              Expression.Call(v, typeof(Lang.Runtime.Value).GetMethod("ToString"))
            ),
            "WriteLine",
            new[] { v }
          )
        );
      }
    }

    
    public static Environment Environment = new Environment(
      new Lang.Runtime.Object("Console", new Dictionary<string, Value>()
      {
        { "WriteLine", Value.MakeFunction(writeLine) },
      })
    );
  }
}
