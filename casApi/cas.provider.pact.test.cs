using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using PactNet;
using PactNet.Infrastructure.Outputters;

using PactNet.Verifier;
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

	public class CasProviderTest
	{

		static readonly string url = "http://localhost:9222";
		public CasProviderTest()
		{
		}

		[Test]
		public void testVerifyPacts()
		{
			var config = new PactVerifierConfig
			{
				LogLevel = PactLogLevel.Information,
				Outputters = new List<IOutput> { new NUnitOutput() },
			};

			string pactPath = @"..\..\..\..\artifacts\pacts/casclient-cas.json";

			var webhost = WebHost.CreateDefaultBuilder()
				.UseStartup<Startup>()
				.ConfigureTestServices(services =>
				{
					services.AddScoped<ICasRepository, MockCasRepository>();

				})
				.UseUrls(url)
				.Build();

			webhost.Start();
			try
			{
				IPactVerifier verifier = new PactVerifier(config);
				verifier
					.ServiceProvider("Events", new Uri(url))
					.WithFileSource(new FileInfo(pactPath))
					.WithRequestTimeout(TimeSpan.FromSeconds(5))
					.WithSslVerificationDisabled()
					.Verify();
			}
			finally
			{
				webhost.StopAsync();
				TestContext.Out.WriteLine("Verification completed.");
			}
		}

	}
}
