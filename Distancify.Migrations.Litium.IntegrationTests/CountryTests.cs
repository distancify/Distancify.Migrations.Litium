using Distancify.Migrations.Litium.IntegrationTests.Asserts;
using Distancify.Migrations.Litium.IntegrationTests.Mocks;
using Distancify.Migrations.Litium.SeedBuilder;
using System.Linq;
using Xunit;

namespace Distancify.Migrations.Litium.IntegrationTests
{
    public class CountryTests
    {
        [Fact]
        public void GenerateFile_CountryWithCurrency_CountrySeedCodeAndCommit()
        {
            // Arrange

            var client = new GraphqlClientMock()
            {
                GraphqlQueryResponse = @"
{
    ""data"": {
        ""countries"": [
            {
                ""id"": ""SE"",
                ""currency"": {
                    ""id"": ""SEK""
                }
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
            Assert.Contains("CountrySeed.Ensure(\"SE\",\"SEK\")", res.Content);
            Assert.DoesNotContain("\t.WithStandardVatRate(", res.Content);
            Assert.Contains("\t.Commit();", res.Content);

        }

        [Fact]
        public void GenerateFile_CountryWithStandardVatRate_CountrySeedCodeAndCommit()
        {
            // Arrange

            var client = new GraphqlClientMock()
            {
                GraphqlQueryResponse = @"
{
    ""data"": {
        ""countries"": [
            {
                ""id"": ""SE"",
                ""standardVatRate"": ""25.000000"",
                ""currency"": {
                    ""id"": ""SEK""
                }
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
            Assert.Contains("CountrySeed.Ensure(\"SE\",\"SEK\")", res.Content);
            Assert.Contains("\t.WithStandardVatRate(25.000000)", res.Content);
            Assert.Contains("\t.Commit();", res.Content);

        }

        [Fact]
        public void GenerateFile_CountryFromMultipleSources_OneCountrySeedCodeAndCommit()
        {
            // Arrange

            var client = new GraphqlClientMock()
            {
                GraphqlQueryResponse = @"
{
    ""data"": {
        ""countries"": [
            {
                ""id"": ""SE"",
                ""standardVatRate"": ""25.000000""
            }
        ],
        ""channels"": [
            {
                ""id"": ""sweden"",
                ""countries"": [
                    {
                        ""id"": ""SE"",
                        ""currency"": {
                            ""id"": ""SEK""
                        }
                    }
                ],
                ""fieldTemplate"": {
                    ""id"": ""channelTemplate1""
                }
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
            Assert.Contains("CountrySeed.Ensure(\"SE\",\"SEK\")", res.Content);
            Assert.Contains("\t.WithStandardVatRate(25.000000)", res.Content);
            Assert.Contains("\t.Commit();", res.Content);
            AssertExtentions.StringCount(1, "CountrySeed.Ensure(", res.Content);

        }
    }
}
