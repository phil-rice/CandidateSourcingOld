namespace xingyi.cas.client
{
    using PactNet.Mocks.MockHttpService;
    using PactNet.Mocks.MockHttpService.Models;
    using Xunit;
    using PactNet.Infrastructure.Outputters;
    using PactNet;
    using xingyi.common;
    using System.Text;

    public class CasClientPact
    {
        private IPactBuilder pactBuilder;
        IMockProviderService _mockProviderService;
        PactVerifier _verifier;
        static string ServiceBaseUri => "http://localhost:9222";
        static IShaCodec _shaCodec = new ShaCodec();
        public CasClient Client => new CasClient(new System.Net.Http.HttpClient(), _shaCodec, ServiceBaseUri);

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

        [Fact]
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
                  Headers = new Dictionary<string, object> { { "Content-Type", "text/plain; charset=utf-8" }},
                  Body = "/cas/someNs/content/h9FJy0JMA4dlbyEdJYn7Wx4WIpkhMJ6YWIQZzMqKc2I"
              });
            byte[] bytes = Encoding.ASCII.GetBytes("somedata");
        var result = await Client.AddItemAsync("someNs", bytes, "application/octet-stream");

        Assert.Equal("/cas/someNs/content/h9FJy0JMA4dlbyEdJYn7Wx4WIpkhMJ6YWIQZzMqKc2I", result);

            _mockProviderService.VerifyInteractions();
            pactBuilder.Build();
        }


}
}
