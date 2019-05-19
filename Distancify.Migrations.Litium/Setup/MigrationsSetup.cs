using System.Reflection;
using Litium.Owin.InversionOfControl;

namespace Distancify.Migrations.Litium.Setup
{
    public class MigrationsSetup : IComponentInstaller
    {
        public void Install(IIoCContainer container, Assembly[] assemblies)
        {
            container.For<IMigrationLog>().UsingFactoryMethod(() => new InMemoryMigrationLog()).RegisterAsSingleton();
            container.For<IMigrationLocator>().UsingFactoryMethod(() => new DefaultMigrationLocator()).RegisterAsSingleton();
            container.For<MigrationService>().RegisterAsSingleton();
        }
    }
}
