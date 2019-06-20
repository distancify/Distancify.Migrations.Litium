using Distancify.Migrations.Litium.Generator;
using Distancify.Migrations.Litium.IntegrationTests.Mocks;
using Xunit;

namespace Distancify.Migrations.Litium.IntegrationTests
{
    public class ChannelTests
    {
        [Fact]
        public void GenerateFile_OneChannelWithoutTemplate_NoSeedsInCode()
        {
            // Arrange

            var client = new GraphqlClientMock()
            {
                GraphqlQueryResponse = @"
{
  ""data"": {
    ""channels"": [
      {
        ""id"": ""sweden""
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
            Assert.DoesNotContain("ChannelSeed.Ensure(\"sweden\"", res.Content);

        }

        [Fact]
        public void GenerateFile_OneChannelWithTemplate_ChannelSeedCode()
        {
            // Arrange

            var client = new GraphqlClientMock()
            {
                GraphqlQueryResponse = @"
{
    ""data"": {
        ""channels"": [
            {
                ""id"": ""sweden"",
                ""fieldTemplate"": {
                    ""id"": ""channelTemplate1"",
                    ""systemId"": ""68c9b08d-1672-4f49-af9f-4a2d0434375c""
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
            Assert.Contains("ChannelSeed.Ensure(\"sweden\", \"channelTemplate1\")", res.Content);
            Assert.Contains(".Commit();", res.Content);

        }
    }
}
