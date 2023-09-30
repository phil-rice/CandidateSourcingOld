using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;
using xingyi.erm;
using xingyi.relationships;
namespace xingyi.erm
{
    using static xingyi.relationships.RelationshipFixture;

    [TestFixture]
    public class PersistRelationshipsTests
    {
        private Mock<IRelationshipUpdater> mockUpdater;
        private Mock<IRelationshipFinder> mockFinder;
        private PersistRelationships persistRelationships;

        private static readonly IRelationshipDefn rel1Defn = new ObjectRelationDefn(e4.store, e4.nameSpace, "rel1Field", r1);
        private static readonly IRelationshipDefn rel2Defn = new ObjectRelationDefn(e5.store, e5.nameSpace, "rel2Field", r2);
        private static readonly List<IRelationshipDefn> relations = new List<IRelationshipDefn> { rel1Defn, rel2Defn };

        private Dictionary<string, object> dict = new Dictionary<string, object> { ["rel1Field"] = e2.name, ["rel2Field"] = e3.name };
        private Dictionary<string, object> dict1changed = new Dictionary<string, object> { ["rel1Field"] = e4.name, ["rel2Field"] = e3.name };
        private Dictionary<string, object> dict12changed = new Dictionary<string, object> { ["rel1Field"] = e4.name, ["rel2Field"] = e5.name };
        [SetUp]
        public void SetUp()
        {
            mockUpdater = new Mock<IRelationshipUpdater>();
            mockFinder = new Mock<IRelationshipFinder>();
            persistRelationships = new PersistRelationships(mockUpdater.Object, mockFinder.Object);
        }

        [Test]
        public void TestCalculatesReplacementRelationsProperly()
        {
            var result = IRelationshipDefn.findAllRelationshipsIn(relations, e1, dict12changed);
            Assert.AreEqual(2, result.Count());
            Assert.AreEqual(rel114, result[0]);
            Assert.AreEqual(rel125, result[1]);

        }
        [Test]
        public async Task TestUpdateAddsAndRemovesRelationshipsCorrectly_Dict_unchanged()
        {
            // Arrange
            var existingRels = new List<Relationship> { rel112, rel123 };
            var newRels = existingRels;
            mockFinder.Setup(f => f.findAll(e1)).Returns(Task.FromResult(existingRels));

            // Act
            var result = await persistRelationships.update(relations, e1, dict);

            // Assert
            // Here you can verify if the right methods on the mock objects were called, and check the result
            mockUpdater.Verify(u => u.set(It.IsAny<Relationship>()), Times.Exactly(0));
            mockUpdater.Verify(u => u.delete(It.IsAny<Relationship>()), Times.Exactly(0));

            Assert.That(result, Is.EquivalentTo(newRels));
        }


        [Test]
        public async Task TestUpdateAddsAndRemovesRelationshipsCorrectly_Changing_Dict12Changed()
        {
            // Arrange
            var existingRels = new List<Relationship> { rel112, rel123 };
            var newRels = new List<Relationship> { rel114, rel125 };
            mockFinder.Setup(f => f.findAll(e1)).Returns(Task.FromResult(existingRels));

            // Act
            var result = await persistRelationships.update(relations, e1, dict12changed);

            // Assert
            // Here you can verify if the right methods on the mock objects were called, and check the result
            mockUpdater.Verify(u => u.set(rel114), Times.Exactly(1));
            mockUpdater.Verify(u => u.set(rel125), Times.Exactly(1));
            mockUpdater.Verify(u => u.delete(rel112), Times.Exactly(1));
            mockUpdater.Verify(u => u.delete(rel123), Times.Exactly(1));

            Assert.That(result, Is.EquivalentTo(newRels));
        }
        [Test]
        public async Task TestUpdateAddsAndRemovesRelationshipsCorrectly_Changing_Dict1Changed()
        {
            // Arrange
            var existingRels = new List<Relationship> { rel112, rel123 };
            var newRels = new List<Relationship> { rel114, rel123 };
            mockFinder.Setup(f => f.findAll(e1)).Returns(Task.FromResult(existingRels));

            // Act
            var result = await persistRelationships.update(relations, e1, dict1changed);

            // Assert
            // Here you can verify if the right methods on the mock objects were called, and check the result
            mockUpdater.Verify(u => u.set(rel114), Times.Exactly(1));
            mockUpdater.Verify(u => u.delete(rel112), Times.Exactly(1));
            mockUpdater.Verify(u => u.set(It.IsAny<Relationship>()), Times.Exactly(1));
            mockUpdater.Verify(u => u.delete(It.IsAny<Relationship>()), Times.Exactly(1));

            Assert.That(result, Is.EquivalentTo(newRels));
        }

        // You can add more tests to cover other scenarios
    }

}
