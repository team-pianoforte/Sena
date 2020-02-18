using System;
using Xunit;
using System.IO;
using System.Text;
using System.Linq;

namespace Pianoforte.Solfege.Lang.Test
{
  public class LookaheadListTest
  {

    [Fact]
    public void TestLookaheadList()
    {

      var list = new LookaheadList<int>(3);

      list[0] = 1;
      list[1] = 2;
      list[2] = 3;

      Assert.Equal(1, list.Next());
      Assert.Equal(2, list.Next());
      Assert.Equal(3, list.Next());
      Assert.Equal(1, list.Next());

      list[3] = 4;
      Assert.Equal(4, list[0]);
      Assert.Equal(4, list.Head);

      list.Clear();
      Assert.Equal(0, list[0]);
      Assert.Equal(0, list[1]);
      Assert.Equal(0, list[2]);
      Assert.Equal(0, list[3]);
    }
  }
}
