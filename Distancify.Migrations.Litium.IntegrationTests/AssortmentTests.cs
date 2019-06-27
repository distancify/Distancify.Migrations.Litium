using Distancify.Migrations.Litium.IntegrationTests.Mocks;
using Distancify.Migrations.Litium.SeedBuilder;
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
            var config = ConfigurationReader.ReadConfiguration(LitiumMigrationGeneratorTests.ExampleConfiguration)[0];

            // Act
            var res = sut.GenerateApplyCode(config);

            // Assert
            Assert.Contains("AssortmentSeed.Ensure(\"assortment1\")", res.Content);
            Assert.Contains("\t.Commit();", res.Content);

        }
    }
}
