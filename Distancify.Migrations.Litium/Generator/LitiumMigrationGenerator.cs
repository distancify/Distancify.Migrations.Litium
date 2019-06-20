using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using YamlDotNet.Serialization.NamingConventions;

namespace Distancify.Migrations.Litium.Generator
{
    public class LitiumMigrationGenerator
    {
        private IGraphqlClient graphqlClient;

        public LitiumMigrationGenerator(IGraphqlClient graphqlClient)
        {
            this.graphqlClient = graphqlClient;
        }

        public MigrationConfiguration[] ReadConfigurationFromFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                Console.WriteLine($"Could not find {filePath}");
                return null;
            }

            return ReadConfiguration(File.ReadAllText(filePath));
        }

        public MigrationConfiguration[] ReadConfiguration(string yamlContent)
        {
            var deserializer = new YamlDotNet.Serialization.DeserializerBuilder()
                .WithNamingConvention(new CamelCaseNamingConvention())
                .Build();

            return deserializer.Deserialize<MigrationConfiguration[]>(yamlContent);
        }

        public GeneratedFile[] GenerateAllFiles(MigrationConfiguration[] configurations)
        {
            List<GeneratedFile> files = new List<GeneratedFile>();
            foreach (var config in configurations)
            {
                var file = GenerateFile(config);
                if (file == null)
                {
                    continue;
                }

                files.Add(file);
            }

            return files.ToArray();
        }

        public GeneratedFile GenerateFile(MigrationConfiguration configuration)
        {
            var responseContainer = graphqlClient.FetchFromGraphql(configuration);
            if (responseContainer == null)
            {
                throw new NullReferenceException("Data object from GraphQL is null, something might be wrong with the query");
            }

            List<ISeed> seeds = new List<ISeed>();
            responseContainer.Data.PopulateSeedsWithData(seeds);

            if (seeds.Count == 0)
            {
                return null;
            }

            var builder = new StringBuilder();
            builder.AppendLine("using Distancify.Migrations.Litium;");
            builder.AppendLine();
            builder.AppendLine($"namespace {configuration.Namespace}");
            builder.AppendLine("{");
            builder.AppendLine($"\tpublic class {configuration.ClassName} : {configuration.BaseMigration}");
            builder.AppendLine("\t{");
            builder.AppendLine("\t\tpublic override void Apply()");
            builder.AppendLine("\t\t{");
            foreach (var seed in seeds)
            {
                builder.Append(seed.GenerateMigration());
            }

            builder.AppendLine("\t\t}");
            builder.AppendLine("\t}");
            builder.AppendLine("}");

            return new GeneratedFile() { Filepath = configuration.Output, Content = builder.ToString()};
        }

    }
}
