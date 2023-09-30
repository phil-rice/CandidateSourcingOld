using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
namespace common.test
{
    public static class Assertions
    {
        public static void ListsEqual<T>(List<T>? l1, List<T>? l2)
        {
            Assert.AreEqual(l1.Count, l2.Count);
            for (int i = 0; i < l1.Count; i++)
            {
                Assert.AreEqual(l1[i], l2[i], $"Item {i}\nExpected {l1[i]}\nActual  {l2[i]}");
            }
        }
    }
}
