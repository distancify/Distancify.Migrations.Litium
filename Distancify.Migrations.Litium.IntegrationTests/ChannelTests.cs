using Distancify.Migrations.Litium.IntegrationTests.Mocks;
using Distancify.Migrations.Litium.SeedBuilder;
using System;
using Xunit;

namespace Distancify.Migrations.Litium.IntegrationTests
{
    public class ChannelTests
    {
        [Fact]
        public void GenerateFile_OneChannelWithoutTemplate_ExceptionDueToMissingTemplate()
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

            // Act + Assert
            Assert.Throws<NullReferenceException>(() => sut.GenerateFile(config));
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
            Assert.Contains("\t.Commit();", res.Content);
        }

        [Fact]
        public void GenerateFile_OneChannelWithTemplateAndOneDomainNameLink_ChannelSeedCode()
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
                    ""id"": ""channelTemplate1""
                },
                ""domains"": [
                    {
                    ""domain"": {
                      ""id"": ""distancify.com""
                        },
                        ""redirect"": false,
                        ""urlPrefix"": null
                    }
                ],
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
            Assert.Contains("\t.WithDomainNameLink(\"distancify.com\")", res.Content);
            Assert.Contains("\t.Commit();", res.Content);
        }

        [Fact]
        public void GenerateFile_OneChannelWithTemplateAndTwoDomainNameLink_ChannelSeedCode()
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
                    ""id"": ""channelTemplate1""
                },
                ""domains"": [
                    {
                    ""domain"": {
                      ""id"": ""distancify.com""
                        },
                        ""redirect"": false,
                        ""urlPrefix"": null
                    },                    
                    {
                    ""domain"": {
                      ""id"": ""distancify.se""
                        },
                        ""redirect"": false,
                        ""urlPrefix"": null
                    }
                ],
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
            Assert.Contains("\t.WithDomainNameLink(\"distancify.com\")", res.Content);
            Assert.Contains("\t.WithDomainNameLink(\"distancify.se\")", res.Content);
            Assert.Contains("\t.Commit();", res.Content);
        }
    }
}
