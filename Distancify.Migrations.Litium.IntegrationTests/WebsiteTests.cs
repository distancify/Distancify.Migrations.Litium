using Distancify.Migrations.Litium.IntegrationTests.Mocks;
using Distancify.Migrations.Litium.SeedBuilder;
using Xunit;

namespace Distancify.Migrations.Litium.IntegrationTests
{
    public class WebsiteTests
    {
        [Fact]
        public void GenerateFile_OneWebsiteWithFieldTemplate_WebsiteSeedCodeAndCommit()
        {
            // Arrange

            var client = new GraphqlClientMock()
            {
                GraphqlQueryResponse = @"
{
    ""data"": {
        ""websites"": [
            {
                ""id"": ""website1"",
                ""fieldTemplate"": {
                    ""id"": ""website1template""
                }
            }
        ]
    }
}"
            };

            
            var config = ConfigurationReader.ReadConfiguration(LitiumMigrationGeneratorTests.ExampleConfiguration)[0];
            var sut = new LitiumMigrationGenerator(client);

            // Act
            var res = sut.GenerateApplyCode(config);

            // Assert
            Assert.Contains("WebsiteSeed.Ensure(\"website1\",\"website1template\")", res.Content);
            Assert.Contains("\t.Commit();", res.Content);

        }
    }
}
