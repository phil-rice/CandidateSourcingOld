using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xingyi.erm;
using xingyi.relationships;

namespace xingi.erm.tests
{
    [TestFixture]
    public class RelationshipDefnTests
    {
        [Test]
        public void TestListRelationDefn()
        {
            // Arrange
            var listRelation = new ListRelationDefn("relationshipList", new List<IRelationshipDefn> {
                new SubjectRelationDefn("store1", "namespace1", "field1", new Relation("namespace1", "relationName1")),
                new ObjectRelationDefn("store2", "namespace2", "field2", new Relation("namespace2", "relationName2"))
            });

            var entity = new Entity("storeMain", "mainNamespace", "mainName");
            var dict = new Dictionary<string, object>
            {
                { "relationshipList", new List<Dictionary<string, object>> {
                    new Dictionary<string, object> { {"field1", "relationValue1" }, {"field2", "relationValue2" } }
                } }
            };

            // Expected Relationships
            var expectedRelation1 = Relationship.from(
                new Entity("store1", "namespace1", "relationValue1"),
                new Relation("namespace1", "relationName1"),
                entity
            );

            var expectedRelation2 = Relationship.from(
                entity,
                new Relation("namespace2", "relationName2"),
                new Entity("store2", "namespace2", "relationValue2")
            );

            // Act
            var result = listRelation.findRelationsIn(entity, dict);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(2));
            Assert.IsTrue(result.Any(r => r.Equals(expectedRelation1)));
            Assert.IsTrue(result.Any(r => r.Equals(expectedRelation2)));
        }
        [Test]
        public void TestSubjectRelationDefn()
        {
            // Arrange
            var subjectRelationDefn = new SubjectRelationDefn(
                "storeSubject", "namespaceSubject", "fieldSubject", new Relation("namespaceRel", "relationNameSubject"));

            var entity = new Entity("storeMain", "mainNamespace", "mainName");
            var dict = new Dictionary<string, object>
    {
        { "fieldSubject", "relationValueSubject" }
    };

            // Expected Relationship
            var expectedRelation = Relationship.from(
                new Entity("storeSubject", "namespaceSubject", "relationValueSubject"),
                new Relation("namespaceRel", "relationNameSubject"),
                entity
            );

            // Act
            var result = subjectRelationDefn.findRelationsIn(entity, dict);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(1));
            Assert.IsTrue(result.Any(r => r.Equals(expectedRelation)));
        }


        [Test]
        public void TestObjectRelationDefn()
        {
            // Arrange
            var objectRelation = new ObjectRelationDefn("storeTest", "namespaceTest", "fieldTest", new Relation("namespaceRel", "relationNameTest"));

            var entity = new Entity("storeMain", "mainNamespace", "mainName");
            var dict = new Dictionary<string, object>
    {
        { "fieldTest", "objectNameTest" }
    };

            // Expected Relationship
            var expectedRelation = Relationship.from(
                entity,
                new Relation("namespaceRel", "relationNameTest"),
                new Entity("storeTest", "namespaceTest", "objectNameTest")
            );

            // Act
            var result = objectRelation.findRelationsIn(entity, dict);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(1));
            Assert.IsTrue(result.Any(r => r.Equals(expectedRelation)));
        }
    }
}

