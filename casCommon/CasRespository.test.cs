using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System.Linq;
using System.Threading.Tasks;

namespace xingyi.cas.common
{
    public class CasRepositoryTests
    {
        private CasDbContext _dbContext;
        private ICasRepository _repository;

        static ContentItem ci1 = new ContentItem("someNs", "someSha1", "someMimeType", new byte[] { 1, 2, 3 });
        static ContentItem ci2 = new ContentItem("someNs", "someSha2", "someMimeType", new byte[] { 1, 2, 3 });
        static ContentItem ci3 = new ContentItem("someNs", "someSha3", "someMimeType", new byte[] { 1, 2, 3 });
        static ContentItem ci4 = new ContentItem("someNs", "someSha4", "someMimeType", new byte[] { 1, 2, 3 });
        static ContentItem co1 = new ContentItem("otherNs", "someSha1", "someMimeType", new byte[] { 1, 2, 3 });
        static ContentItem co2 = new ContentItem("otherNs", "someSha2", "someMimeType", new byte[] { 1, 2, 3 });

        static List<ContentItem> cis = new List<ContentItem> { ci1, ci2, ci3, ci4 };
        static List<ContentItem> cos = new List<ContentItem> { co1, co2 };


        [SetUp]
        public void Setup()
        {
            // Set up in-memory database
            var options = new DbContextOptionsBuilder<CasDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb") // This gives each test a separate in-memory database
                .Options;

            _dbContext = new CasDbContext(options);
            _dbContext.Database.EnsureCreated(); // Ensure database is created

            _repository = new CasRepository(_dbContext); // Assuming your concrete ICasRepository implementation is named CasRepository
        }

        [TearDown]
        public void TearDown()
        {
            _dbContext.Database.EnsureDeleted(); // Cleanup in-memory database after test
            _dbContext.Dispose();
        }

        [Test]
        public async Task Add_ShouldSaveContentItem()
        {

            await _repository.Àdd(ci1);
            await _dbContext.saveAllChanges();

            var savedItem = await _dbContext.ContentItems.FindAsync(ci1.Namespace, ci1.SHA);
            Assert.Equals(ci1, savedItem);
        }

        [Test]
        public async Task ContentItem_ShouldReturnCorrectItem()
        {
            _dbContext.ContentItems.Add(ci1);
            await _dbContext.saveAllChanges();

            var fetchedItem = await _repository.ContentItem(ci1.Namespace, ci1.SHA);

            Assert.Equals(ci1, fetchedItem);
        }

        [Test]
        public async Task ContentItems_ShouldReturnExpectedNumberOfItems()
        {
            _dbContext.ContentItems.Add(ci1);
            _dbContext.ContentItems.Add(ci2);
            _dbContext.ContentItems.Add(ci3);
            _dbContext.ContentItems.Add(ci4);
            _dbContext.ContentItems.Add(co1);
            _dbContext.ContentItems.Add(co2);
            await _dbContext.saveAllChanges();


            var foundCis = (await _repository.ContentItems(ci1.Namespace, 2)).ToList();
            var foundCos = (await _repository.ContentItems(co1.Namespace, 2)).ToList();

            // Assert
            Assert.AreEqual(2, foundCis.Count());
            Assert.Contains(foundCis[0], cis);
            Assert.Contains(foundCis[1], cis);


            Assert.AreEqual(2, foundCos.Count());
            Assert.Contains(foundCos[0], cos);
            Assert.Contains(foundCos[1], cos);
        }

        [Test]
        public async Task ContentItems_ShouldReturnAllItems_WhenCountIsGreaterThanTotal()
        {
            // Seed the in-memory database with some data
            _dbContext.ContentItems.Add(ci1);
            _dbContext.ContentItems.Add(ci2);
            _dbContext.ContentItems.Add(ci3);
            await _dbContext.saveAllChanges();

            // Act
            var foundCis = (await _repository.ContentItems(ci1.Namespace, 10)).ToList();

            Assert.AreEqual(3, foundCis.Count());
            Assert.Contains(foundCis[0], cis);
            Assert.Contains(foundCis[1], cis);
            Assert.Contains(foundCis[2], cis);
        }

    }
}
