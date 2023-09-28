using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xingyi.relationships;

namespace xingyi.relationships
{
    using static RelationshipFixture;
    [TestFixture]
    public class RelationshipRepositoryTests
    {
        private RelationshipDbContext _context;
        private RelationshipRepository _repository;

        [SetUp]
        public void Setup()
        {
            // Set up the in-memory database. Each time it creates a fresh database for isolation.
            var options = new DbContextOptionsBuilder<RelationshipDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new RelationshipDbContext(options);
            _context.Database.EnsureCreated();

            _repository = new RelationshipRepository(_context);
        }

        [TearDown]
        public void Teardown()
        {
            _context.Database.EnsureDeleted();
        }


        [Test]
        public async Task TestFindBySubjectAnd()
        {
            // Arrange
            _context.Relationships.Add(Relationship.from(e1, r1, e2));
            _context.Relationships.Add(Relationship.from(e1, r1, e3));
            _context.Relationships.Add(Relationship.from(e2, r1, e4));
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.findBySubjectAnd(e1, r1);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(e2, result[0].obj());
            Assert.AreEqual(e3, result[1].obj());
        }

        [Test]
        public async Task TestFindBySubjectAndObjectWhenIn()
        {
            // Arrange
            _context.Relationships.Add(Relationship.from(e1, r1, e2));
            _context.Relationships.Add(Relationship.from(e1, r1, e3));
            _context.Relationships.Add(Relationship.from(e2, r1, e4));
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.findBySubjectAndObject(e1, r1, e3);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(e1, result.subject());
            Assert.AreEqual(r1, result.relation());
            Assert.AreEqual(e3, result.obj());
        }
        [Test]
        public async Task TestFindBySubjectAndObjectWhenNotIn()
        {
            // Arrange

            // Act
            var result = await _repository.findBySubjectAndObject(e1, r2, e3);

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public async Task TestFindByObject()
        {
            _context.Relationships.Add(Relationship.from(e1, r1, e2));
            _context.Relationships.Add(Relationship.from(e1, r1, e3));
            _context.Relationships.Add(Relationship.from(e4, r1, e2));
            await _context.SaveChangesAsync();


            // Act
            var result = await _repository.findByObject(e2);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(e1, result[0].subject());
            Assert.AreEqual(e4, result[1].subject());
        }

        [Test]
        public async Task TestFindByObjectAnd()
        {
            // Arrange
            _context.Relationships.Add(Relationship.from(e1, r1, e2));
            _context.Relationships.Add(Relationship.from(e1, r1, e3));
            _context.Relationships.Add(Relationship.from(e4, r1, e2));
            _context.Relationships.Add(Relationship.from(e3, r2, e2));
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.findByObjectAnd(e2, r1);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(e1, result[0].subject());
            Assert.AreEqual(e4, result[1].subject());
        }

        [Test]
        public async Task TestSetWhenNotPresent()
        {
            // Arrange
            await _repository.set(Relationship.from(e1, r1, e2));
            var result = await _repository.findBySubjectAndObject(e1, r1, e2);
            Assert.IsNotNull(result);
        }

        [Test]
        public async Task TestSetWhenPresent()
        {
            _context.Relationships.Add(Relationship.from(e1, r1, e2));
            await _context.SaveChangesAsync();

            await _repository.set(Relationship.from(e1, r1, e2));
            var result = await _repository.findBySubject(e1);
            Assert.AreEqual(1, result.Count());
        }

        [Test]
        public async Task TestUpdate()
        {
            _context.Relationships.Add(Relationship.from(e1, r1, e2));
            await _context.SaveChangesAsync();

            var toEdit = await _repository.findBySubjectAndObject(e1, r1, e2);
            toEdit.RelationshipNamespace = r2.nameSpace;
            toEdit.RelationshipName = r2.name;
            await _repository.update(toEdit);
            var result = await _repository.findBySubject(e1);
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual(r2, result[0].relation());
        }


    }
}
