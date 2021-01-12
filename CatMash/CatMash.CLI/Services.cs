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
                string credentialsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "catmash-plml.json");
                FirestoreClientBuilder builder = new () { CredentialsPath = credentialsPath };
                
                return builder.Build();
            });

            serviceProvider = services.BuildServiceProvider();
        }

        public static T Get<T>() => serviceProvider.GetService<T>() ?? throw new InvalidOperationException($"Missing service '{typeof(T).Name}'");
    }
}