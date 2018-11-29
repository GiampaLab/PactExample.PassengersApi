using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using PactNet;
using PactNet.Infrastructure.Outputters;
using System.Collections.Generic;
using Xunit;
using Xunit.Abstractions;

namespace ProviderTests
{
    public class ProviderTests
    {
        private readonly ITestOutputHelper output;

        public ProviderTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void EnsureProviderApiHonoursPactWithConsumer()
        {
            //Arrange
            var config = new PactVerifierConfig
            {
                Outputters = new List<IOutput>
                {
                    new XUnitOutput(output)
                },
                Verbose = true,
                ProviderVersion = "0.0.1"
            };

            const string providerUrl = "http://localhost:5001";

            WebHost.CreateDefaultBuilder(new string[0])
                .UseStartup<TestStartup>()
                .UseUrls(providerUrl)
                .Build().Start();

            IPactVerifier pactVerifier = new PactVerifier(config);
            pactVerifier
                .ProviderState($"{providerUrl}/provider-states")
                .ServiceProvider("Provider", providerUrl)
                .HonoursPactWith("Consumer")
                .PactUri("..\\..\\..\\..\\..\\Consumer\\ConsumerTests\\pacts\\consumer-provider.json")
                //.PactUri("http://pactbroker.optimum-development.nl/pacts/provider/Provider/consumer/Consumer/latest")
                .Verify();
        }
    }
}
