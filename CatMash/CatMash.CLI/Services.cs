using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore.V1;
using Grpc.Core;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CatMash.CLI
{
    internal static class Services
    {
        private static readonly IServiceProvider serviceProvider;

        static Services()
        {
            IServiceCollection services = new ServiceCollection();

            services.AddTransient<IDbSeeder, DbSeeder>();

            services.AddSingleton(sp =>
            {
#if DEBUG
                string credPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../../../IgnoredFiles/catmash-plml.json");
                FirestoreClientBuilder builder = new() { CredentialsPath = credPath };
#elif RELEASE
                string credentialsJson = Environment.GetEnvironmentVariable("GcpCredentialsJson") ?? throw new InvalidOperationException("Missing 'GcpCredentialsJson' environment variable");
                FirestoreClientBuilder builder = new() { JsonCredentials = credentialsJson };
#else
#error Unexpected configuration
#endif

                return builder.Build();
            });

            serviceProvider = services.BuildServiceProvider();
        }

        public static T Get<T>() => serviceProvider.GetService<T>() ?? throw new InvalidOperationException($"Missing service '{typeof(T).Name}'");
    }
}