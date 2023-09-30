using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace common
{
    using NUnit.Framework;
    using System.Collections.Generic;
    using xingyi.common;

    [TestFixture]
    public class ListsTests
    {
        [Test]
        public void GetListDifferences_Test()
        {
            // Arrange
            var oldList = new List<string> { "a", "b", "c" };
            var newList = new List<string> { "b", "c", "d", "e" };

            // Act
            var (added, removed) = Lists.GetListDifferences(oldList, newList);

            // Assert
            Assert.That(added, Is.EquivalentTo(new List<string> { "d", "e" }));
            Assert.That(removed, Is.EquivalentTo(new List<string> { "a" }));
        }
        public void GetListDifferences_OrderDoesntMatterTest()
        {
            // Arrange
            var oldList = new List<string> { "a", "b", "c" };
            var newList = new List<string> { "c","b", "d", "e" };

            // Act
            var (added, removed) = Lists.GetListDifferences(oldList, newList);

            // Assert
            Assert.That(added, Is.EquivalentTo(new List<string> { "d", "e" }));
            Assert.That(removed, Is.EquivalentTo(new List<string> { "a" }));
        }
    }

}
