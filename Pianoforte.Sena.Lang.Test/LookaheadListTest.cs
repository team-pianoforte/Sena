using System;
using Xunit;
using System.IO;
using System.Text;
using System.Linq;

namespace Pianoforte.Sena.Lang.Test
{
  public class LookaheadListTest
  {

    [Fact]
    public void TestLookaheadList()
    {

      var list = new LookaheadList<int>(3);

      Assert.Throws<IndexOutOfRangeException>(() =>
      {
        var _ = list.First;
      });

      Assert.Equal(1, list.Push(1));
      Assert.Equal(2, list.Push(2));
      Assert.Equal(3, list.Push(3));

      Assert.Equal(1, list.First);
      Assert.Equal(1, list.Lookup(0));
      Assert.Equal(2, list[1]);
      Assert.Equal(3, list.Lookup(2));
      Assert.Equal(2, list.Lookup(4));
      Assert.Equal(1, list.Lookup(6));
    }
  }
}
