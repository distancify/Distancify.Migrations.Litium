using System.Configuration;
using System.Reflection;
using Litium.Owin.InversionOfControl;

namespace Distancify.Migrations.Litium.Setup
{
    public class MigrationsSetup : IComponentInstaller
    {
        public void Install(IIoCContainer container, Assembly[] assemblies)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["FoundationConnectionString"].ConnectionString;

            if (!string.IsNullOrWhiteSpace(connectionString))
            {
                container.For<IMigrationLog>().UsingFactoryMethod(() => new SqlServerMigrationLog(connectionString)).RegisterAsSingleton();
            }
            else
            {
                container.For<IMigrationLog>().UsingFactoryMethod(() => new InMemoryMigrationLog()).RegisterAsSingleton();
            }

            container.For<IMigrationLocator>().UsingFactoryMethod(() => new DefaultMigrationLocator()).RegisterAsSingleton();
            container.For<MigrationService>().RegisterAsSingleton();
        }
    }
}
