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
    private Dictionary<string, Value> Members { get; }
    public Object(string name) : this(name, new Dictionary<string, Value>()) { }

    public Object(string name, Dictionary<string, Value> members)
    {
      Name = name;
      Members = members;
    }


    public override string ToString()
    {
      return string.Format("object {0}", Name);
    }

    public void SetMember(string name, Value value)
    {
      Members[name] = value;
    }

    public Value Member(string name)
    {
      if (Members.TryGetValue(name, out var value))
      {
        return value;
      }
      throw new RuntimeException(string.Format(Properties.Resources.UndefinedMember, Name, name));
    }
  }
}
