using Distancify.Migrations.Litium.Generator;
using Distancify.Migrations.Litium.IntegrationTests.Mocks;
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

            var sut = new LitiumMigrationGenerator(client);
            var config = sut.ReadConfiguration(LitiumMigrationGeneratorTests.ExampleConfiguration)[0];

            // Act
            var res = sut.GenerateFile(config);

            // Assert
            Assert.Contains("WebsiteSeed.Ensure(\"website1\",\"website1template\")", res.Content);
            Assert.Contains(".Commit();", res.Content);

        }
    }
}
