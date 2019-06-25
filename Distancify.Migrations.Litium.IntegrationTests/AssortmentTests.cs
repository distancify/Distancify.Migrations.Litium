using Distancify.Migrations.Litium.Generator;
using Distancify.Migrations.Litium.IntegrationTests.Mocks;
using Xunit;

namespace Distancify.Migrations.Litium.IntegrationTests
{
    public class AssortmentTests
    {
        [Fact]
        public void GenerateFile_OneAssortment_AssortmentSeedCodeAndCommit()
        {
            // Arrange

            var client = new GraphqlClientMock()
            {
                GraphqlQueryResponse = @"
{
    ""data"": {
        ""assortments"": [
            {
                ""id"": ""assortment1""
            }
        ]
    }
}"
            };

            var sut = new LitiumMigrationGenerator(client);
            var config = sut.ReadConfiguration(LitiumMigrationGeneratorTests.ExampleConfiguration)[0];

            // Act
            var res = sut.GenerateFile(config);

            // Assert
            Assert.Contains("AssortmentSeed.Ensure(\"assortment1\")", res.Content);
            Assert.Contains("\t.Commit();", res.Content);

        }
    }
}
