
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace xingyi.relationships
{
    using NUnit.Framework;
    using Moq;
    using System.Threading.Tasks;
    using System.Text;
    using xingyi.cas;
    using xingyi.cas.client;
    using xingyi.events.client;
    using System.Net.Mime;
    using xingyi.cas.common;

    [TestFixture]
    public class DefaultEntityToJsonTests
    {
        private Mock<ICasGetter> mockCasGetter;
        private Mock<IProcessedEventsGetter> mockEventsGetter;
        private DefaultEntityToJson entityToJson;

        [SetUp]
        public void SetUp()
        {
            mockCasGetter = new Mock<ICasGetter>();
            mockEventsGetter = new Mock<IProcessedEventsGetter>();
            entityToJson = new DefaultEntityToJson(mockCasGetter.Object, mockEventsGetter.Object);
        }


        [Test]
        public async Task GetJsonAsync_EntityIsLiterallStore_ReturnsString()
        {
            var entity = new Entity(DefaultEntityToJson.literalStore, "email", "test@email.com");
            var result = await entityToJson.GetJsonAsync<string>(entity);
            Assert.AreEqual("test@email.com", result);
        }

        [Test]
        public async Task GetJsonAsync_EntityIsCasStore_ReturnsDeserializedJson()
        {
            var entity = new Entity(DefaultEntityToJson.casStore, "testName", "testNamespace");
            var jsonData = new Dictionary<string, object> { ["key"] = "value" };
            var jsonString = System.Text.Json.JsonSerializer.Serialize(jsonData);
            mockCasGetter.Setup(x => x.GetItemAsync(entity.nameSpace, entity.name))
                .ReturnsAsync(new ContentItem("nsNotUsed", "shaNotUsed", "application/json", Encoding.UTF8.GetBytes(jsonString)));

            var result = await entityToJson.GetJsonAsync<Dictionary<string, string>>(entity);
            Assert.AreEqual("value", result["key"]);
        }

        [Test]
        public async Task GetJsonAsync_EntityIsEventStore_ReturnsProcessedEvents()
        {
            var entity = new Entity(DefaultEntityToJson.eventStore, "testName", "testNamespace");
            var expectedEvents = new Dictionary<string, object> { { "processed", "events" } };
            mockEventsGetter.Setup(x => x.GetProcessedEventsAsync(entity.nameSpace, entity.name))
                .ReturnsAsync(expectedEvents);

            var result = await entityToJson.GetJsonAsync<Dictionary<string, string>>(entity);
            Assert.AreEqual("events", result["processed"]);
        }

        [Test]
        public async Task GetJsonAsync_EntityIsCasStoreButNotJson_ThrowsException()
        {
            var entity = new Entity(DefaultEntityToJson.casStore, "testName", "testNamespace");
            mockCasGetter.Setup(x => x.GetItemAsync(entity.nameSpace, entity.name))
                     .ReturnsAsync(new ContentItem("nsNotUsed", "shaNotUsed", "text/plain", Encoding.UTF8.GetBytes("something")));

            var ex = Assert.ThrowsAsync<Exception>(() => entityToJson.GetJsonAsync<string>(entity));
            StringAssert.StartsWith("Have cas that isn't json", ex.Message);
        }

        [Test]
        public async Task GetJsonAsync_EntityIsCasStoreButDataIsNull_ThrowsException()
        {
            var entity = new Entity(DefaultEntityToJson.casStore, "testName", "testNamespace");
            mockCasGetter.Setup(x => x.GetItemAsync(entity.nameSpace, entity.name)).ReturnsAsync((ContentItem)null);

            var ex = Assert.ThrowsAsync<Exception>(() => entityToJson.GetJsonAsync<string>(entity));
            StringAssert.StartsWith("Entity not found", ex.Message);
        }

        [Test]
        public async Task GetJsonAsync_UnknownStore_ThrowsException()
        {
            var entity = new Entity("unknown", "testName", "testNamespace");

            var ex = Assert.ThrowsAsync<Exception>(() => entityToJson.GetJsonAsync<string>(entity));
            StringAssert.StartsWith("Unknown store", ex.Message);
        }

    }
}