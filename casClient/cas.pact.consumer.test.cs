namespace xingyi.cas.client
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

    public class CasClientPact
    {
        private IPactBuilder pactBuilder;
        IMockProviderService _mockProviderService;
        PactVerifier _verifier;
        static string ServiceBaseUri => "http://localhost:9222";
        static IShaCodec _shaCodec = new ShaCodec();
        public CasClient Client => new CasClient(new DefaultHttpClient(new System.Net.Http.HttpClient(), ServiceBaseUri), _shaCodec);

        public CasClientPact()
        {
            this.pactBuilder = new PactBuilder(new PactConfig
            {
                SpecificationVersion = "2.0.0",
                PactDir = @"..\..\..\..\artifacts\pacts",
                LogDir = @"..\..\..\..\artifacts\pactlogs"
            }).ServiceConsumer("CasClient").HasPactWith("Cas");


            _mockProviderService = pactBuilder.MockService(9222);

        }

        [OneTimeTearDown]
        public void makePactFilesAtEnd()
        {
            pactBuilder.Build();
            _mockProviderService.Stop();
        }


        [Test]
        async public Task EnsureCasClientHandlesAddItemEndpoint()
        {
            _mockProviderService
              .UponReceiving("A request to add item")
              .With(new ProviderServiceRequest
              {
                  Method = HttpVerb.Post,
                  Path = "/cas/someNs",
                  Headers = new Dictionary<string, object>
                  {
                  { "Content-Type", "application/octet-stream" }
                  },
                  Body = "somedata"
              })
              .WillRespondWith(new ProviderServiceResponse
              {
                  Status = 201,
                  Headers = new Dictionary<string, object> { { "Content-Type", "text/plain; charset=utf-8" } },
                  Body = "/cas/someNs/content/h9FJy0JMA4dlbyEdJYn7Wx4WIpkhMJ6YWIQZzMqKc2I"
              });
            byte[] bytes = Encoding.ASCII.GetBytes("somedata");
            var result = await Client.AddItemAsync("someNs", bytes, "application/octet-stream");

            Assert.AreEqual("/cas/someNs/content/h9FJy0JMA4dlbyEdJYn7Wx4WIpkhMJ6YWIQZzMqKc2I", result);

            _mockProviderService.VerifyInteractions();
        
        }

        [Test]
        async public Task EnsureCasClientHandlesAddItemEndpointWhenAlreadyIn()
        {
            _mockProviderService
              .UponReceiving("A request to add item that is already stored")
              .With(new ProviderServiceRequest
              {
                  Method = HttpVerb.Post,
                  Path = "/cas/someNs",
                  Headers = new Dictionary<string, object>
                  {
                  { "Content-Type", "application/octet-stream" }
                  },
                  Body = "alreadyin"
              })
              .WillRespondWith(new ProviderServiceResponse
              {
                  Status = 200,
                  Headers = new Dictionary<string, object> { { "Content-Type", "text/plain; charset=utf-8" } },
                  Body = "/cas/someNs/content/-AFBB8Hv7uIEEmV1Srn0Y-OkfnMplM-FX8TEh4-SucM"
              });
            byte[] bytes = Encoding.ASCII.GetBytes("alreadyin");
            var result = await Client.AddItemAsync("someNs", bytes, "application/octet-stream");

            Assert.AreEqual("/cas/someNs/content/-AFBB8Hv7uIEEmV1Srn0Y-OkfnMplM-FX8TEh4-SucM", result);

            _mockProviderService.VerifyInteractions();
            
        }

        [Test]
        async public Task EnsureCasClientHandlesGetEndpointWhenIn()
        {
            _mockProviderService
              .UponReceiving("A request to get item that is in the store")
              .With(new ProviderServiceRequest
              {
                  Method = HttpVerb.Get,
                  Path = "/cas/someNs/content/-AFBB8Hv7uIEEmV1Srn0Y-OkfnMplM-FX8TEh4-SucM"
              })
              .WillRespondWith(new ProviderServiceResponse
              {
                  Status = 200,
                  Headers = new Dictionary<string, object> { { "Content-Type", "text/plain" } },
                  Body = "alreadyin"
              });
            var result = await Client.GetItemAsync("someNs", "-AFBB8Hv7uIEEmV1Srn0Y-OkfnMplM-FX8TEh4-SucM");
            var expected = new ContentItem("someNs", "-AFBB8Hv7uIEEmV1Srn0Y-OkfnMplM-FX8TEh4-SucM", "text/plain", Encoding.ASCII.GetBytes("alreadyin"));
            Assert.AreEqual(expected, result);

            _mockProviderService.VerifyInteractions();

        }
        [Test]
        async public Task EnsureCasClientHandlesGetEndpointWhenNotIn()
        {
            _mockProviderService
              .UponReceiving("A request to get item that is not in the store")
              .With(new ProviderServiceRequest
              {
                  Method = HttpVerb.Get,
                  Path = "/cas/someNs/content/notIn"
              })
              .WillRespondWith(new ProviderServiceResponse
              {
                  Status = 404
              });
            byte[] bytes = Encoding.ASCII.GetBytes("somedata");
            var result = await Client.GetItemAsync("someNs","notIn");

            Assert.IsNull( result);

            _mockProviderService.VerifyInteractions();
        }
    }
}
