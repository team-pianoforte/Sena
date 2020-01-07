using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Linq;
using System.Linq.Expressions;

namespace Pianoforte.Sena.Lang.Runtime
{

  public class Object
  {
    public string Name { get; }
    private readonly Dictionary<string, Value> members = new Dictionary<string, Value>();

    public Object(string name)
    {
      Name = name;
    }

    public override string ToString()
    {
      return string.Format("object {0}", Name);
    }

    public void SetMember(string name, Value value)
    {
      members[name] = value;
    }

    public Value Member(string name)
    {
      if (members.TryGetValue(name, out var value))
      {
        return value;
      }
      throw new RuntimeException(string.Format(Properties.Resources.UndefinedMember, Name, name));
    }
  }
}
