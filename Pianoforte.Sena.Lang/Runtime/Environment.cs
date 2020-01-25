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
  }
}
