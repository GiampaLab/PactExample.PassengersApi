using System;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace Provider
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ItemsContext>(options =>
                options.UseInMemoryDatabase("TestingDB"));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public virtual void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.Map("/api/providerValues", builder =>
            {
                builder.Run(async context =>
                {
                    using (var serviceScope = app.ApplicationServices.CreateScope())
                    {
                        var itemsContext = serviceScope.ServiceProvider.GetRequiredService<ItemsContext>();
                        if (itemsContext.Items.SingleOrDefault(i => i.Id == 1) == null)
                        {
                            itemsContext.Add(new Item
                            {
                                Id = 1,
                                Value = "Some Value"
                            });
                            itemsContext.SaveChanges();
                        }
                        var id = Convert.ToInt32(context.Request.Path.Value.Substring(context.Request.Path.Value.LastIndexOf('/') + 1));
                        context.Response.ContentType = "application/json; charset=utf-8";
                        await context.Response.WriteAsync(JsonConvert.SerializeObject(itemsContext.Items.FirstOrDefault(i => i.Id == id)));
                    }
                });
            });
            app.Run(async (context) =>
            {
                await context.Response.WriteAsync("Hello World!");
            });
        }
    }
}