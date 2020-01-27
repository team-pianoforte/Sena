using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Pianoforte.Sena.Lang.Runtime
{
  public class Environment
  {
    public Library Library { get; }
    public Block RootBlock =>
      new Block(null, new Dictionary<string, Value>(
        Library.Objects.Select((obj) =>
          new KeyValuePair<string, Value>(obj.Name, Value.MakeObject(obj)))
        )
      );

    public Environment(Library lib)
    {
      Library = lib;
    }
  }
}
