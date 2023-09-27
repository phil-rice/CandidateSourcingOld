using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using PactNet;
using xingyi.cas.common;
using xingyi.common;
using NUnit.Framework;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using PactNet.Infrastructure.Outputters;
using Microsoft.AspNetCore;



namespace casApi
{


    public class NUnitOutput : IOutput
    {
        public void WriteLine(string line)
        {
            Console.WriteLine(line);
            TestContext.WriteLine(line);
        }
    }

    public class CasProviderTest : IDisposable
    {
        private IWebHost webhost;

        static readonly string url = "http://localhost:9222";
        public CasProviderTest()
        {

            webhost = WebHost.CreateDefaultBuilder()
            .UseStartup<Startup>() // Your regular Startup class
            .ConfigureTestServices(services =>
            {
                services.AddScoped<ICasRepository, MockCasRepository>();

            })
            .UseUrls(url)
            .Build();

            webhost.Start();
        }

        public void Dispose()
        {
            webhost?.Dispose();
        }

        [Test]
        public void EnsureCasAPIHonoursPactWithConsumer()
        {
            IPactVerifier pactVerifier = new PactVerifier(new PactVerifierConfig
            {
                Outputters = new List<IOutput> { new NUnitOutput() },
                Verbose = true // Output verbose verification logs to the test output
            });


            var path = Path.GetFullPath(Path.Combine(".", @"..\..\..\..\artifacts\pacts"));
            Console.WriteLine($"Path: {path}");

            pactVerifier.ServiceProvider("Cas", url)
                        .HonoursPactWith("CasClient")
                        .PactUri(path + "/casclient-cas.json")
                        .Verify();

            TestContext.Out.WriteLine("Verification completed.");
        }
    }
}
