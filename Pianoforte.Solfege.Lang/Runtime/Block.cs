﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Linq.Expressions;
using System.Collections;

namespace Pianoforte.Solfege.Lang.Runtime
{
  public class Block
  {
    private readonly Dictionary<string, Value> variables;
    public Block Parent { get; }
    public Block() : this(null) { }

    public Block(Block parent) : this(parent, new Dictionary<string, Value>()) { }

    public Block(Block parent, Dictionary<string, Value> vars)
    {
      Parent = parent;
      variables = vars;
    }

    public Block Root
    {
      get
      {
        var v = this;
        while (v.Parent != null)
        {
          v = v.Parent;
        }
        return v;
      }
    }

    public Value GetVariable(string name)
    {
      if (variables.TryGetValue(name, out var v))
      {
        return v;
      }
      if (Parent != null)
      {
        return Parent.GetVariable(name);
      }
      throw new RuntimeException(string.Format(Properties.Resources.UndefinedVariable, name));
    }
    public void SetVariable(string name, Value v)
    {
      variables[name] = v;
    }

    public Block NewInnerBlock()
    {
      return new Block(this);
    }
  }
}
