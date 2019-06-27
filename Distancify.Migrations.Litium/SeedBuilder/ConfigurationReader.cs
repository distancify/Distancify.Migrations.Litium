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
                Console.WriteLine($"Could not find {filePath}");
                return null;
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
