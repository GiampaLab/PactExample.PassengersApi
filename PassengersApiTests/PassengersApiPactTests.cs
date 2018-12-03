using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using PactNet;
using PactNet.Infrastructure.Outputters;
using System.Collections.Generic;
using Xunit;
using Xunit.Abstractions;

namespace PassengersApiTests
{
    public class PassengersApiPactTests
    {
        private readonly ITestOutputHelper output;

        public PassengersApiPactTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void EnsurePassengersApiHonoursPactWithFlightsApi()
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
                .ServiceProvider("PassengersApi", providerUrl)
                .HonoursPactWith("FlightsApi")
                //.PactUri("..\\..\\..\\..\\..\\FlightsApi\\FlightsApiTests\\pacts\\flightsapi-passengersapi.json")
                .PactUri("http://pactbroker.optimum-development.nl/pacts/provider/PassengersApi/consumer/FlightsApi/latest")
                .Verify();
        }
    }
}
