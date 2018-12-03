using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using PassengersApi;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace PassengersApiTests
{
    public class TestStartup : Startup
    {
        private const int passengerId = 5;
        private readonly Dictionary<string, Action<IApplicationBuilder>> providerStates;

        public TestStartup()
        {
            providerStates = new Dictionary<string, Action<IApplicationBuilder>>
            {
                {
                    $"There is a passenger with id {passengerId}",
                    (app) =>
                    {
                        using (var serviceScope = app.ApplicationServices.CreateScope())
                        {
                            var itemsContext = serviceScope.ServiceProvider.GetRequiredService<DatabaseContext>();
                            if (itemsContext.Passengers.SingleOrDefault(i => i.Id == passengerId) == null)
                            {
                                itemsContext.Add(new Passenger
                                {
                                    Id = passengerId,
                                    Name = "Mario",
                                    Surname = "Rossi"
                                });
                                itemsContext.SaveChanges();
                            }
                        }
                    }
                }
            };
        }
        public override void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.MapWhen(context =>
            {
                if (context.Request.Method == HttpMethod.Post.ToString() &&
                    context.Request.Body != null)
                    return true;
                return false;
            }, builder =>
            {
                builder.Run(async handler =>
                {
                    string jsonRequestBody;
                    using (var reader = new StreamReader(handler.Request.Body, Encoding.UTF8))
                    {
                        jsonRequestBody = reader.ReadToEnd();
                    }

                    var providerState = JsonConvert.DeserializeObject<ProviderState>(jsonRequestBody);

                    //A null or empty provider state key must be handled
                    if (providerState != null &&
                        !string.IsNullOrEmpty(providerState.State) &&
                        providerState.Consumer == "FlightsApi")
                    {
                        providerStates[providerState.State].Invoke(app);
                    }

                    await handler.Response.WriteAsync(string.Empty);
                });
            });
            base.Configure(app, env);
        }
    }
}
