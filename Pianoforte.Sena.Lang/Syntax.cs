using System;
using System.Collections.Generic;
using System.Text;
using System.Linq.Expressions;

namespace Pianoforte.Sena.Lang
{
  public static class Syntax
  {
    public static Expression Literal(Token token)
    {
      return Expression.Constant(Runtime.Value.FromToken(token));
    }
    
    public static Expression BinaryExpr(Expression lhs, Token op, Expression rhs)
    {
      if (!op.IsBinaryOp())
      {
      }
      var method = op.Kind switch
      {
        TokenKind.OpPlus => "Add",
        _ => throw new InternalAssertionException("Binary operator is required"),
     
        };
      return Expression.Call(null, typeof(Runtime.Operations).GetMethod(method), lhs, rhs);
    }
    
  }
}
