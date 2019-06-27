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
                var file = GenerateApplyCode(config);
                if (file == null)
                {
                    continue;
                }

                files.Add(file);
            }

            return files.ToArray();
        }

        public GeneratedFile GenerateApplyCode(MigrationConfiguration configuration)
        {
            var responseContainer = graphqlClient.FetchFromGraphql(configuration).GetAwaiter().GetResult();
            if (responseContainer == null)
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



            return new GeneratedFile() { Filepath = configuration.Output, Content = builder.ToString() };
        }

    }
}
