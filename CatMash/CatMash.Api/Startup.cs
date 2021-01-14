using CatMash.Api.Services;
using Google.Cloud.Firestore.V1;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using System;
using System.IO;
using System.Text.Json;

namespace CatMash.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        private const string AllowFrontCorsPolicy = "AllowFrontEndPolicy";

        public void ConfigureServices(IServiceCollection services)
        {
#if DEBUG
            string credPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../../../IgnoredFiles/catmash-plml.json");

            services.AddSingleton(sp =>
            {
                FirestoreClientBuilder builder = new() { CredentialsPath = credPath };

                return builder.Build();
            });
#elif RELEASE
            string jsonCreds = Configuration.GetValue<string>("GcpCredentialsJson");

            services.AddSingleton(sp =>
            {
                FirestoreClientBuilder builder = new() { JsonCredentials = jsonCreds };

                return builder.Build();
            });
#endif

            services.AddSingleton<ICatService, CatService>();

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "CatMash.Api", Version = "v1" });
            });

            string frontUrl = Configuration.GetValue<string>("AllowCorsOrigin");
            services.AddCors(options => options.AddDefaultPolicy(
                policy => policy.WithMethods("GET", "POST")
#if DEBUG
                                .AllowAnyOrigin()
#elif RELEASE
                                .WithOrigins(frontUrl) // Can't use cors with localhost
#else
#error Unexpected configuration
#endif
                                .AllowAnyHeader()
                ));
        }


        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors();

            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "CatMash.Api v1"));
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}