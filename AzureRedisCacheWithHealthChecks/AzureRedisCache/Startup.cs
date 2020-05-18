using AzureRedisCache.API.Extensions;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System.Text;

namespace AzureRedisCache.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddCustomSwagger()
                .AddCustomFilters()
                .AddServices()
                .AddCommonServices(Configuration)
                .AddCustomHealthChecks(Configuration);

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
                c.RoutePrefix = string.Empty;
            });

            app.UseHealthChecks("/healthz", new HealthCheckOptions
            {
                Predicate = registration => registration.Name.Equals("self"),
                ResponseWriter = async (context, report) =>
                {
                    context.Response.ContentType = "application/json; charset=utf-8";
                    var bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(report));
                    await context.Response.Body.WriteAsync(bytes);
                }
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHealthChecks("healthz", new HealthCheckOptions()
                {
                    Predicate = _ => true,
                    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                });

                endpoints.MapHealthChecksUI(setup =>
                {
                    setup.UIPath = "/healthchecks-ui"; // this is ui path in your browser
                    setup.ApiPath = "/health-ui-api"; // the UI ( spa app )  use this path to get information from the store ( this is NOT the healthz path, is internal ui api )
                });

                endpoints.MapControllers();
            });
        }
    }
}
