using System;
using System.Collections.Generic;
using System.Text;

namespace Pianoforte.Sena.Lang.Runtime
{
  public static class Operations
  {
    public static Value Add(Value lhs, Value rhs)
    {
      if (lhs.Type == ValueType.String || rhs.Type == ValueType.String)
      {
        return Value.MakeString(lhs.ToString() + rhs.ToString());
      }
      if (lhs.Type == ValueType.Number && rhs.Type == ValueType.Number)
      {
        return Value.MakeNumber(lhs.Number + rhs.Number);
      }
      throw new RuntimeException(string.Format(Properties.Resources.InvalidAddition, lhs, rhs));
    }

    public static Value MemberAccess(Value receiver, string name)
    {
      if (receiver.Type != ValueType.Object)
      {
        throw new RuntimeException(Properties.Resources.NonObjectMemberAccess);
      }
      return receiver.Object.Member(name);
    }
  }
}
