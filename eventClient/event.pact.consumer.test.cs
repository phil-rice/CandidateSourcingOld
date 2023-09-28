namespace xingyi.events.client
{
	using NUnit.Framework;
	using PactNet.Infrastructure.Outputters;
	using PactNet;
	using xingyi.common;
	using System.Text;
	using xingyi.common.http;
	using xingyi.cas.common;
	using static xingi.events.tests.EventFixture;


	public class EventClientPact
	{
		private IPactBuilderV2 pact;

		static string ServiceBaseUri => "http://localhost:9223";
		public EventClient Client;



		public EventClientPact()
		{
			var httpClient = new System.Net.Http.HttpClient();
			httpClient.BaseAddress = new Uri(ServiceBaseUri);
			Client = new EventClient(new DefaultHttpClient(httpClient));


			var config = new PactConfig
			{
				PactDir = @"..\..\..\..\artifacts\pacts"
			};

			IPactV2 pact = Pact.V2("EventsClient", "Events", config);

			this.pact = pact.WithHttpInteractions(9223);


		}

		[OneTimeTearDown]
		public void makePactFilesAtEnd()
		{
		}


		[Test]
		async public Task TestGetProcessedEventsAsyncWhenNoEvents()
		{
			pact
		   .UponReceiving("A request to get processed events when no events")
		   .WithRequest(HttpMethod.Get, "/events/someNs/noEvents")
		   .WillRespond()
		   .WithStatus(404);
			await pact.VerifyAsync(async ctx =>
			{
				var result = await Client.GetProcessedEventsAsync("someNs", "noEvents");
				Assert.AreEqual(0, result.Count);
			});
		}
		[Test]
		async public Task TestGetProcessedEventsAsyncWhenEvents()
		{
			pact
				.UponReceiving("A request to get processed events when 2 events")
				.WithRequest(HttpMethod.Get, "/events/someNs/someEvents")
				.WillRespond()
					.WithJsonBody(processed12)
					.WithStatus(200);
			await pact.VerifyAsync(async ctx =>
			{
				var result = await Client.GetProcessedEventsAsync("someNs", "someEvents");
				Assert.AreEqual(1, result.Count);
				Assert.AreEqual(2, result["count"]);
			});

		}
		[Test]
		async public Task TestGetEventsAsyncWhenNoEvents()
		{
			pact
				.UponReceiving("A request to get events when no events")
				.WithRequest(HttpMethod.Get, "/events/someNs/noEvents/raw")
				.WillRespond()
					.WithStatus(404);
			await pact.VerifyAsync(async ctx =>
			{
				var result = await Client.GetEventsAsync("someNs", "noEvents");
				Assert.AreEqual(0, result.Count);
			});
		}
		[Test]
		async public Task TestGetEventsAsyncWhenEvents()
		{
			pact
				.UponReceiving("A request to get events when 2 events")
				.WithRequest(HttpMethod.Get, "/events/someNs/someEvents/raw")
				.WillRespond()
					.WithStatus(200)
					.WithJsonBody(ev12);
			await pact.VerifyAsync(async ctx =>
			{
				var result = await Client.GetEventsAsync("someNs", "someEvent");
				Assert.AreEqual(2, result.Count());
				Assert.AreEqual(ev12[0], result[0]);
				Assert.AreEqual(ev12[1], result[1]);
			});
		}



		[Test]
		async public Task AddItem()
		{
			pact
				.UponReceiving("A request to add item that is already stored")
				.WithRequest(HttpMethod.Post, "/events/someNs/someEvent")
				.WillRespond()
					.WithStatus(200)
					.WithJsonBody(processed123);
			await pact.VerifyAsync(async ctx =>
			{
				var result = await Client.GetEventsAsync("someNs", "someEvent");
				Assert.Fail("write");
			});

		}
	}
}
