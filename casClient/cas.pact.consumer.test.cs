namespace xingyi.cas.client
{

    using NUnit.Framework;
    using PactNet.Infrastructure.Outputters;
    using PactNet;
    using xingyi.common;
    using System.Text;
    using xingyi.common.http;
    using xingyi.cas.common;

    public class CasClientPact
    {
        private readonly IPactBuilderV2 pact;
        private readonly int port = 9222;
        static string ServiceBaseUri => "http://localhost:9222";
        static IShaCodec _shaCodec = new ShaCodec();
        public readonly CasClient Client;
        public CasClientPact()
        {
            var httpClient = new System.Net.Http.HttpClient();
            httpClient.BaseAddress = new Uri(ServiceBaseUri);
            this.Client = CasClient.forTests(new DefaultHttpClient(httpClient), _shaCodec);

            var config = new PactConfig
            {
                PactDir = @"..\..\..\..\artifacts\pacts"
            };

            IPactV2 pact = Pact.V2("CasClient", "Cas", config);

            this.pact = pact.WithHttpInteractions(9222);
        }

        [OneTimeTearDown]
        public void makePactFilesAtEnd()
        {
        }


        [Test]
        async public Task EnsureCasClientHandlesAddItemEndpoint()
        {
            pact
              .UponReceiving("A request to add item")
              .WithRequest(HttpMethod.Post, "/cas/someNs")
              .WithBody("somedata", "application/octet-stream")
              .WillRespond()
              .WithStatus(201)
              .WithBody("/cas/someNs/content/h9FJy0JMA4dlbyEdJYn7Wx4WIpkhMJ6YWIQZzMqKc2I", "text/plain; charset=utf-8");

            await pact.VerifyAsync(async ctx =>
            {
                byte[] bytes = Encoding.ASCII.GetBytes("somedata");
                var result = await Client.AddItemAsync("someNs", bytes, "application/octet-stream");
                Assert.AreEqual("/cas/someNs/content/h9FJy0JMA4dlbyEdJYn7Wx4WIpkhMJ6YWIQZzMqKc2I", result);

            });

        }

        [Test]
        async public Task EnsureCasClientHandlesAddItemEndpointWhenAlreadyIn()
        {
            pact
           .UponReceiving("A request to add item that is already stored")
           .WithRequest(HttpMethod.Post, "/cas/someNs")
           .WithBody("alreadyin", "application/octet-stream")
           .WillRespond()
           .WithStatus(200)
           .WithBody("/cas/someNs/content/-AFBB8Hv7uIEEmV1Srn0Y-OkfnMplM-FX8TEh4-SucM", "text/plain; charset=utf-8");

            await pact.VerifyAsync(async ctx =>
            {
                byte[] bytes = Encoding.ASCII.GetBytes("alreadyin");
                var result = await Client.AddItemAsync("someNs", bytes, "application/octet-stream");
                Assert.AreEqual("/cas/someNs/content/-AFBB8Hv7uIEEmV1Srn0Y-OkfnMplM-FX8TEh4-SucM", result);
            });
        }

        [Test]
        async public Task EnsureCasClientHandlesGetEndpointWhenIn()
        {
            pact
         .UponReceiving("A request to get item that is in the store")
         .WithRequest(HttpMethod.Get, "/cas/someNs/content/-AFBB8Hv7uIEEmV1Srn0Y-OkfnMplM-FX8TEh4-SucM")
         .WillRespond()
         .WithStatus(200)
         .WithBody("alreadyin", "text/plain; charset=utf-8");

            await pact.VerifyAsync(async ctx =>
            {
                var result = await Client.GetItemAsync("someNs", "-AFBB8Hv7uIEEmV1Srn0Y-OkfnMplM-FX8TEh4-SucM");
                var expected = new ContentItem("someNs", "-AFBB8Hv7uIEEmV1Srn0Y-OkfnMplM-FX8TEh4-SucM", "text/plain", Encoding.ASCII.GetBytes("alreadyin"));
                Assert.AreEqual(expected, result);
            });
        }
        [Test]
        async public Task EnsureCasClientHandlesGetEndpointWhenNotIn()
        {
            pact.UponReceiving("A request to get item that is not in the store")
    .WithRequest(HttpMethod.Get, "/cas/someNs/content/notIn")
    .WillRespond()
    .WithStatus(404);

            await pact.VerifyAsync(async ctx =>
            {
                byte[] bytes = Encoding.ASCII.GetBytes("somedata");
                var result = await Client.GetItemAsync("someNs", "notIn");

                Assert.IsNull(result);
            });
        }
    }
}
