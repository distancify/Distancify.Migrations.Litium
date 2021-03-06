# Distancify.Migrations.Litium

This project contains a fluid API to aid in generating data in Litium together with Distancify.Migrations. It also provides the possibility to generate migration seeds based on a Litium instance.

## Getting Started

### Install

```powershell
Install-Package Distancify.Migrations.Litium
```

## Seeding a Litium installation

The library revolves around a set of ISeed implementation. Each ISeed implementation provides a method called `Ensure`. This method __ensures__ that the given entity exists in Litium. If the entity already exist, nothing happens. A seed can also verify given properties of the entity.

Example usage:

```csharp
ChannelSeed.Ensure("MyChannel", "DefaultChannelFieldTemplate")
	.WithMarketId("B2C")
	.WithField("Flag", "sv.png")
	.Commit();
```

_Note_ the last method call `Commit()`. This method creates/updates the entity. Nothing will happen if you forget this call.

### Litium Bootstrapping

If you base your project on a completely empty Litium database, there are some basic bootstrapping that
needs to be made, such as creating the special _System_ user. By inheriting from `LitiumMigration` instead
of `Distancify.Migrations.Migration`, this will be automatically taken care of for you.

### Apply migrations at Litium startup

If you have a set of migrations that you want to run on all environments, you can make sure these are applied everywhere by creating a startup task:

```csharp
using Distancify.Migrations;
using Distancify.Migrations.Litium;
using Litium;
using Litium.Foundation;
using Litium.Owin.Lifecycle;
using System.Collections.Generic;
using System.Reflection;

public class MigrationsSetup : IPostSetupTask
{
    public void PostSetup(IEnumerable<Assembly> assemblies)
    {
        using (Solution.Instance.SystemToken.Use())
        {
            IoC.Resolve<MigrationService>().Apply<ProductionMigration>();

            if (bool.TryParse(ConfigurationManager.AppSettings["RunDevMigrationAtStartup"], out bool result) && result)
            {
                IoC.Resolve<MigrationService>().Apply<DevelopmentMigration>();
            }
        }
    }
}
```

## Generating migrations seeds

### Configuration

The seeds are based on the content of a yaml configuration, the configuration can contain several migrations, targeting different Litium instances and write to different files.

The file configuration file should be named  ``migrationConfiguration.yml`` and should be located in the same folder as the project file.

```yaml
--- 
- id: Migration1
  baseMigration: DevelopmentMigration
  className: TestMigration1
  host: http://localhost:56666
  namespace: Eqquo.Litium.Migrations.Production.Development
  output: c:\temp\migration\test1.cs
  query: |
      query{
          channels{
              id,
              countries{
                  id,
                  currencies{
                      id
                  }
              }
          }
      }
- id: Migration2
  baseMigration: DevelopmentMigration
  className: TestMigration2
  host: http://localhost:56666
  namespace: Eqquo.Litium.Migrations.Production.Development
  output: c:\temp\migration\test2.cs
  query: |
      query{
          channels{
              id
          }
      }
```

The source code project contains an example of this configuration file.

### Generate Litium migration

The generation is done by a powershell commandlet. The module is imported by the init.ps1 in the nuget package.

You can invoke the LitiumMigration commandlet in the Package Manager Console.

```powershell
Push-LitiumMigration -ConfigFileName C:\temp\migration\test.yml
```

The ``ConfigFileName`` is optional, and if not provided is the default value: *migrationConfiguration.yml*

## Running the tests

The tests are built using xUnit and does not require any setup in order to run inside Visual Studio's standard test runner.

Currently does the project only contain tests for the generation of migration seeds.

## Contributing

Please read [CONTRIBUTING.md](CONTRIBUTING.md) for details on our code of conduct, and the process for submitting pull requests to us.

## Versioning

We use [SemVer](http://semver.org/) for versioning.

## Publishing

Use CreateRelease.ps1 to create a new release. There's a CI build from every __master__ build on Distancify's internal NuGet feed.

## Authors

See the list of [contributors](https://github.com/distancify/Distancify.Migrations.Litium/graphs/contributors) who participated in this project.

## License

This project is licensed under the LGPL v3 License - see the [LICENSE](LICENSE) file for details