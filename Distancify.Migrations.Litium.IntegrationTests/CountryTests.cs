using Distancify.Migrations.Litium.Generator;
using Distancify.Migrations.Litium.IntegrationTests.Mocks;
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
            var config = sut.ReadConfiguration(LitiumMigrationGeneratorTests.ExampleConfiguration)[0];

            // Act
            var res = sut.GenerateFile(config);

            // Assert
            Assert.Contains("CountrySeed.Ensure(\"SE\",\"SEK\")", res.Content);
            Assert.DoesNotContain("CountrySeed.WithStandardVatRate(", res.Content);
            Assert.Contains(".Commit();", res.Content);

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
            var config = sut.ReadConfiguration(LitiumMigrationGeneratorTests.ExampleConfiguration)[0];

            // Act
            var res = sut.GenerateFile(config);

            // Assert
            Assert.Contains("CountrySeed.Ensure(\"SE\",\"SEK\")", res.Content);
            Assert.Contains("CountrySeed.WithStandardVatRate(25)", res.Content);
            Assert.Contains(".Commit();", res.Content);

        }
    }
}
