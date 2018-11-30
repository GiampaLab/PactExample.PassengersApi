using System;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace PassengersApi
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<DatabaseContext>(options =>
                options.UseInMemoryDatabase("TestingDB"));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public virtual void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            using (var serviceScope = app.ApplicationServices.CreateScope())
            {
                var databaseContext = serviceScope.ServiceProvider.GetRequiredService<DatabaseContext>();
                if (databaseContext.Passengers.SingleOrDefault(i => i.Id == 1) == null)
                {
                    databaseContext.Add(new Passenger
                    {
                        Id = 1,
                        Name = "Marco",
                        Surname = "Rossi"
                    });
                    databaseContext.SaveChanges();
                }
            }
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.Map("/api/passengers", builder =>
            {
                builder.Run(async context =>
                {
                    using (var serviceScope = app.ApplicationServices.CreateScope())
                    {
                        var databaseContext = serviceScope.ServiceProvider.GetRequiredService<DatabaseContext>();
                        var id = Convert.ToInt32(context.Request.Path.Value.Substring(context.Request.Path.Value.LastIndexOf('/') + 1));
                        context.Response.ContentType = "application/json; charset=utf-8";
                        await context.Response.WriteAsync(JsonConvert.SerializeObject(databaseContext.Passengers.FirstOrDefault(i => i.Id == id)));
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