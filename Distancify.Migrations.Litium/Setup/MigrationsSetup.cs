using Litium.Runtime;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Configuration;
using System.Reflection;

namespace Distancify.Migrations.Litium.Setup
{
    public class MigrationsSetup : IApplicationConfiguration
    {
        public void Configure(ApplicationConfigurationBuilder app)
        {

            app.ConfigureServices(services =>
            {
                var connectionString = ConfigurationManager.ConnectionStrings["FoundationConnectionString"].ConnectionString;

                if (!string.IsNullOrWhiteSpace(connectionString))
                {
                    services.AddSingleton<IMigrationLogFactory>((serviceProvider) => new SqlServerMigrationLogFactory(connectionString));
                }
                else
                {
                    services.AddSingleton<IMigrationLogFactory>((serviceProvider) => new InMemoryMigrationLogFactory());
                }

                services.AddSingleton<IMigrationLocator>((serviceProvider) => new DefaultMigrationLocator());
                services.AddSingleton<IMigrationFactory>((serviceProvider) => new MigrationFactory());
                services.AddSingleton<MigrationService>();
            });
        }
    }
}
