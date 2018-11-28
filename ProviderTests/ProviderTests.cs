using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using PactNet;
using PactNet.Infrastructure.Outputters;
using Provider;
using System.Collections.Generic;
using Xunit;
using Xunit.Abstractions;

namespace ProviderTests
{
    public class ProviderTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly ITestOutputHelper output;
        private readonly CustomWebApplicationFactory factory;

        public ProviderTests(ITestOutputHelper output, CustomWebApplicationFactory factory)
        {
            this.output = output;
            this.factory = factory;
        }

        [Fact]
        public void EnsureProviderApiHonoursPactWithConsumer()
        {
            //Arrange
            var config = new PactVerifierConfig
            {
                Outputters = new List<IOutput> //NOTE: We default to using a ConsoleOutput, however xUnit 2 does not capture the console output, so a custom outputter is required.
                {
                    new XUnitOutput(output)
                },
                CustomHeader = new KeyValuePair<string, string>("Authorization", "Basic VGVzdA=="), //This allows the user to set a request header that will be sent with every request the verifier sends to the provider
                Verbose = true //Output verbose verification logs to the test output
            };

            const string providerUrl = "http://localhost:5001";
            WebHost.CreateDefaultBuilder(new string[0])
                .UseStartup<TestStartup>()
                .UseUrls(providerUrl)
                .Build().Start();
            //factory.ConfigureWebHost().UseUrls(providerUrl).Build().Start();

            //Act / Assert
            IPactVerifier pactVerifier = new PactVerifier(config);
            pactVerifier
                .ProviderState($"{providerUrl}/provider-states")
                .ServiceProvider("Provider", providerUrl)
                .HonoursPactWith("Consumer")
                .PactUri("consumer-provider.json")
                ////or
                //.PactUri("http://pact-broker/pacts/provider/Something%20Api/consumer/Consumer/latest") //You can specify a http or https uri
                //                                                                                       //or
                //.PactUri("http://pact-broker/pacts/provider/Something%20Api/consumer/Consumer/latest", new PactUriOptions("someuser", "somepassword")) //You can also specify http/https basic auth details
                .Verify();
        }
    }
}
