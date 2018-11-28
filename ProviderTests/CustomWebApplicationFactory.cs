using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Provider;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace ProviderTests
{
    public class CustomWebApplicationFactory
    {
        private Dictionary<string, Action<IApplicationBuilder>> providerStates;

        public CustomWebApplicationFactory()
        {
            providerStates = new Dictionary<string, Action<IApplicationBuilder>>
            {
                {
                    "There is an item with id 5",
                    (app) =>
                    {
                        using (var serviceScope = app.ApplicationServices.CreateScope())
                        {
                            var itemsContext = serviceScope.ServiceProvider.GetRequiredService<ItemsContext>();
                            if (itemsContext.Items.SingleOrDefault(i => i.Id == 5) == null)
                            {
                                itemsContext.Add(new Item
                                {
                                    Id = 5,
                                    Value = "Some Value"
                                });
                                itemsContext.SaveChanges();
                            }
                        }
                    }
                }
            };
        }
        public IWebHostBuilder ConfigureWebHost()
        {
            return WebHost.CreateDefaultBuilder(new string[0]).Configure(appBuilder =>
            {
                appBuilder.MapWhen(context =>
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
                            providerState.Consumer == "Consumer")
                        {
                            providerStates[providerState.State].Invoke(appBuilder);
                        }

                        await handler.Response.WriteAsync(string.Empty);
                    });
                });
            }).UseStartup<Startup>();
        }
    }
}
