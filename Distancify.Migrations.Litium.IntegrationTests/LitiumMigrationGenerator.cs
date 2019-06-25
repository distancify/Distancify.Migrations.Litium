using Distancify.Migrations.Litium.IntegrationTests.Mocks;
using Distancify.Migrations.Litium.SeedBuilder;
using Xunit;

namespace Distancify.Migrations.Litium.IntegrationTests
{
    public class LitiumMigrationGeneratorTests
    {
        [Fact]
        public void GenerateAllFiles_EmptyDataResponse_NoFiles()
        {
            // Arrange

            var client = new GraphqlClientMock()
            {
                GraphqlQueryResponse = @"{""data"": {}}"
            };

            var sut = new LitiumMigrationGenerator(client);
            var configs = sut.ReadConfiguration(ExampleConfiguration);

            // Act
            var res = sut.GenerateAllFiles(configs);

            // Assert
            Assert.Empty(res);

        }


        public const string ExampleConfiguration = @"---
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
...";
    }
}
