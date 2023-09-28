namespace xingyi.events.client
{
    using PactNet.Mocks.MockHttpService;
    using PactNet.Mocks.MockHttpService.Models;
    using NUnit.Framework;
    using PactNet.Infrastructure.Outputters;
    using PactNet;
    using xingyi.common;
    using System.Text;
    using xingyi.common.http;
    using xingyi.cas.common;

    public class EventClientPact
    {
        private IPactBuilder pactBuilder;
        IMockProviderService _mockProviderService;
        PactVerifier _verifier;
        static string ServiceBaseUri => "http://localhost:9223";
        public EventClient Client => new EventClient(new DefaultHttpClient(new System.Net.Http.HttpClient(), ServiceBaseUri));

        static Event ev1 = new SetToCasEvent("namespace1", "sha1");
        static Event ev2 = new SetFieldToValueEvent("field1", "value1");
        static Event ev3 = new SetFieldToCasEvent("field2", "namespace2", "sha2");
        static List<Event> ev12 = new List<Event> { ev1, ev2 };
        static List<Event> ev123 = new List<Event> { ev1, ev2, ev3 };

        public EventClientPact()
        {
            this.pactBuilder = new PactBuilder(new PactConfig
            {
                SpecificationVersion = "2.0.0",
                PactDir = @"..\..\..\..\artifacts\pacts",
                LogDir = @"..\..\..\..\artifacts\pactlogs"
            }).ServiceConsumer("EventsClient").HasPactWith("Events");


            _mockProviderService = pactBuilder.MockService(9223);

        }

        [OneTimeTearDown]
        public void makePactFilesAtEnd()
        {
            pactBuilder.Build();
            _mockProviderService.Stop();
        }


        [Test]
        async public Task TestGetProcessedEventsAsyncWhenNoEvents()
        {
            _mockProviderService
              .UponReceiving("A request to get processed events when no events")
              .With(new ProviderServiceRequest
              {
                  Method = HttpVerb.Get,
                  Path = "/events/someNs/noEvents"
              })
              .WillRespondWith(new ProviderServiceResponse
              {
                  Status = 404
              });
            var result = await Client.GetProcessedEventsAsync("someNs", "noEvents");
            Assert.AreEqual(0, result.Count);

            _mockProviderService.VerifyInteractions();
        }
        [Test]
        async public Task TestGetProcessedEventsAsyncWhenEvents()
        {
            _mockProviderService
              .UponReceiving("A request to get processed events when 2 events")
              .With(new ProviderServiceRequest
              {
                  Method = HttpVerb.Get,
                  Path = "/events/someNs/someEvent"
              })
              .WillRespondWith(new ProviderServiceResponse
              {
                  Status = 200,
                  Headers = new Dictionary<string, object> { { "Content-Type", "application/json" } },
                  Body = @"{""count"":2}"

              });
            var result = await Client.GetProcessedEventsAsync("someNs", "someEvent");
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(2, result["count"]);
            _mockProviderService.VerifyInteractions();
        }
        [Test]
        async public Task TestGetEventsAsyncWhenNoEvents()
        {
            _mockProviderService
              .UponReceiving("A request to get events when no events")
              .With(new ProviderServiceRequest
              {
                  Method = HttpVerb.Get,
                  Path = "/events/someNs/noEvents/raw"
              })
              .WillRespondWith(new ProviderServiceResponse
              {
                  Status = 404
              });
            var result = await Client.GetEventsAsync("someNs", "noEvents");
            Assert.AreEqual(0, result.Count);

            _mockProviderService.VerifyInteractions();
        }
        [Test]
        async public Task TestGetEventsAsyncWhenEvents()
        {
            _mockProviderService
              .UponReceiving("A request to get events when 2 events")
              .With(new ProviderServiceRequest
              {
                  Method = HttpVerb.Get,
                  Path = "/events/someNs/someEvent/raw"
              })
              .WillRespondWith(new ProviderServiceResponse
              {
                  Status = 200,
                  Headers = new Dictionary<string, object> { { "Content-Type", "application/json" } },
                  Body = @"{""count"":2}"
              }); ;
            var result = await Client.GetEventsAsync("someNs", "someEvent");
            Assert.AreEqual(2, result.Count());
            Assert.AreEqual(ev12[0], result[0]);
            Assert.AreEqual(ev12[1], result[1]);

            _mockProviderService.VerifyInteractions();
        }



        [Test]
        async public Task AddItem()
        {
            _mockProviderService
              .UponReceiving("A request to add item that is already stored")
              .With(new ProviderServiceRequest
              {
                  Method = HttpVerb.Post,
                  Path = "/events/someNs/someEvent",
                  Headers = new Dictionary<string, object>
                  {
                  { "Content-Type", "application/json" }
                  },
                  Body = Event.eventToJson(ev3)
              })
              .WillRespondWith(new ProviderServiceResponse
              {
                  Status = 200,
                  Headers = new Dictionary<string, object> { { "Content-Type", "application/json" } },
                  Body = @"{""count"":3}"
              });
            var result = await Client.AddEvent("someNs", "someEvent", ev3);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(3, result["count"]);
            _mockProviderService.VerifyInteractions();

        }
    }
}
