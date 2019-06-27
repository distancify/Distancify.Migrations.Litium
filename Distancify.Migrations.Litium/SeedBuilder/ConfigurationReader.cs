using System;
using System.IO;
using YamlDotNet.Serialization.NamingConventions;

namespace Distancify.Migrations.Litium.SeedBuilder
{
    public static class ConfigurationReader
    {
        public static MigrationConfiguration[] ReadConfigurationsFromFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"Migration configuration file located at {filePath} could not be found",filePath);
            }

            return ReadConfiguration(File.ReadAllText(filePath));
        }

        public static MigrationConfiguration[] ReadConfiguration(string yamlContent)
        {
            var deserializer = new YamlDotNet.Serialization.DeserializerBuilder()
                .WithNamingConvention(new CamelCaseNamingConvention())
                .Build();

            return deserializer.Deserialize<MigrationConfiguration[]>(yamlContent);
        }
    }
}
