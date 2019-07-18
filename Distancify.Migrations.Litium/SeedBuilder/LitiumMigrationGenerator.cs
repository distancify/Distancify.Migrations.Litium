using Scriban;
using System;
using System.Collections.Generic;
using System.Text;

namespace Distancify.Migrations.Litium.SeedBuilder
{
    public class LitiumMigrationGenerator
    {
        private IGraphqlClient graphqlClient;

        public LitiumMigrationGenerator(IGraphqlClient graphqlClient)
        {
            this.graphqlClient = graphqlClient;
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
            var responseContainer = graphqlClient.FetchFromGraphql(configuration).GetAwaiter().GetResult();
            if (responseContainer == null || responseContainer.Data == null)
            {
                throw new NullReferenceException("Data object from GraphQL is null, something might be wrong with the query");
            }

            var seedRepository = new Generator();
            seedRepository.PopulateSeedsWithData(responseContainer.Data);

            if (seedRepository.NumberOfSeeds == 0)
            {
                return null;
            }

            var builder = new StringBuilder();
            builder.Append(seedRepository.GenerateMigration());

            var template = Template.Parse(@"
using System;
using Distancify.Migrations.Litium.Seeds.Globalization;
using Distancify.Migrations.Litium.Seeds.Products;
using Distancify.Migrations.Litium.Seeds.Websites;
using Distancify.Migrations.Litium.Seeds.Customers;
using Distancify.Migrations.Litium.Seeds.Media;

namespace {{ config.namespace }}
{
	public class {{ config.class_name }} : {{ config.base_migration }}
	{
		public override void Apply()
		{ {{ apply_code }} 
        }
	}
}");

            var content = template.Render(new { Config = configuration, ApplyCode = builder.ToString() });

            return new GeneratedFile() { Filepath = configuration.Output, Content = content };
        }

    }
}
