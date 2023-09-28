using NUnit.Framework;
using System.Text.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using xingyi.cas.client;
using Microsoft.EntityFrameworkCore;
namespace xingyi.events.tests
{
    using static xingi.events.tests.EventFixture;
    using Json = Dictionary<string, object>;


    public class JsonEventExecutorTests
    {
        private class FakeCasJsonGetter : ICasJsonGetter
        {
            public Task<Json> GetJsonAsync(string nameSpace, string sha)
            {
                // Return a predefined json for testing purposes
                return Task.FromResult(new Dictionary<string, object>
                {
                    { "exampleKey", "exampleValue" }
                });
            }
        }

        [Test]
        public async Task SetToCas_ShouldReturnCorrectJson()
        {
            var executor = new JsonEventExecutor(new FakeCasJsonGetter());
            var result = await executor.SetToCas(new SetToCasEvent("testNamespace", "testSha"), new Dictionary<string, object>());

            string expectedJson = "{\"exampleKey\":\"exampleValue\"}";
            string actualJson = JsonSerializer.Serialize(result);

            Assert.AreEqual(expectedJson, actualJson);
        }

        [Test]
        public async Task SetFieldToValue_ShouldSetFieldCorrectly()
        {
            var executor = new JsonEventExecutor(new FakeCasJsonGetter());
            var result = await executor.SetFieldToValue(new SetFieldToValueEvent("testField", "testValue"), new Dictionary<string, object>());

            string expectedJson = "{\"testField\":\"testValue\"}";
            string actualJson = JsonSerializer.Serialize(result);

            Assert.AreEqual(expectedJson, actualJson);
        }

        [Test]
        public async Task SetFieldToCas_ShouldSetFieldWithCasValue()
        {
            var executor = new JsonEventExecutor(new FakeCasJsonGetter());
            var result = await executor.SetFieldToCas(new SetFieldToCasEvent("testField", "testNamespace", "testSha"), new Dictionary<string, object>());

            string expectedJson = "{\"testField\":{\"exampleKey\":\"exampleValue\"}}";
            string actualJson = JsonSerializer.Serialize(result);

            Assert.AreEqual(expectedJson, actualJson);
        }
    }

    public class EventSerialisationTest
    {
        [Test]
        public void ShouldDeserializeJsonToListOfEventsCorrectly()
        {


            string jsonString = Event.listToJson(ev123);

            // Act
            var actualEvents = Event.jsonToList(jsonString); // Adjust the call if the method is not public or static.

            // Assert
            Assert.AreEqual(ev123.Count, actualEvents.Count);
            for (int i = 0; i < ev123.Count; i++)
            {
                Assert.AreEqual(ev123[i], actualEvents[i]);
            }
        }

    }


    public class EventStoreRepositoryTests
    {
        private EventStoreDbContext _dbContext;

        [SetUp]
        public void Setup()
        {
            // Setting up the in-memory database
            var options = new DbContextOptionsBuilder<EventStoreDbContext>()
                .UseInMemoryDatabase(databaseName: "EventStoreDb")
                .Options;

            _dbContext = new EventStoreDbContext(options);
            _dbContext.Database.EnsureCreated();

            // You can seed the in-memory database here if needed
        }

        [TearDown]
        public void TearDown()
        {
            _dbContext.Database.EnsureDeleted();
            _dbContext.Dispose();
        }

        [Test]
        public async Task TestGetAsyncWhenEmpty()
        {
            var repo = new EventStoreRepository(_dbContext);
            var events = await repo.getAsync("testNamespace", "testName");
            Assert.AreEqual(0, events.Count);
        }


        [Test]
        public async Task TestAddAsyncWhenEmpty()
        {
            var repo = new EventStoreRepository(_dbContext);
            var newEvent = new SetToCasEvent("namespace1", "sha1");

            await repo.addAsync("testNamespace", "testName", newEvent);

            var eventsList = await repo.getAsync("testNamespace", "testName");
            Assert.AreEqual(1, eventsList.Count);
            Assert.Contains(newEvent, eventsList);
        }
        [Test]
        public async Task TestAddAsyncTwice()
        {
            var repo = new EventStoreRepository(_dbContext);

            await repo.addAsync("testNamespace", "testName", ev1);
            await repo.addAsync("testNamespace", "testName", ev2);

            var eventsList = await repo.getAsync("testNamespace", "testName");
            Assert.AreEqual(2, eventsList.Count);
            Assert.AreEqual(ev1, eventsList[0]);
            Assert.AreEqual(ev2, eventsList[1]);
        }
    }

}


