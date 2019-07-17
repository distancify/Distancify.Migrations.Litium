using Distancify.Migrations.Litium.IntegrationTests.Mocks;
using Distancify.Migrations.Litium.SeedBuilder;
using Xunit;

namespace Distancify.Migrations.Litium.IntegrationTests
{
    public class CurrencyTests
    {
        [Fact(Skip = "GraphQL structure has been changed completely")]
        public void GenerateFile_OneCurrency_CurrencySeedCodeAndCommit()
        {
            // Arrange

            var client = new GraphqlClientMock()
            {
                GraphqlQueryResponse = @"
{
    ""data"": {
        ""currencies"": [
            {
                ""id"": ""SEK""
            }
        ]
    }
}"
            };

            var sut = new LitiumMigrationGenerator(client);
            var config = ConfigurationReader.ReadConfiguration(LitiumMigrationGeneratorTests.ExampleConfiguration)[0];

            // Act
            var res = sut.GenerateFile(config);

            // Assert
            Assert.Contains("CurrencySeed.Ensure(\"SEK\")", res.Content);
            Assert.Contains("\t.Commit();", res.Content);

        }

        [Fact(Skip = "GraphQL structure has been changed completely")]
        public void GenerateFile_OneBaseCurrency_CurrencySeedCodeAndCommit()
        {
            // Arrange

            var client = new GraphqlClientMock()
            {
                GraphqlQueryResponse = @"
{
    ""data"": {
        ""currencies"": [
            {
                ""id"": ""SEK"",
                ""isBaseCurrency"": true
            }
        ]
    }
}"
            };

            var sut = new LitiumMigrationGenerator(client);
            var config = ConfigurationReader.ReadConfiguration(LitiumMigrationGeneratorTests.ExampleConfiguration)[0];

            // Act
            var res = sut.GenerateFile(config);

            // Assert
            Assert.Contains("CurrencySeed.Ensure(\"SEK\")", res.Content);
            Assert.Contains("\t.IsBaseCurrency(true)", res.Content);
            Assert.Contains("\t.Commit();", res.Content);

        }

        [Fact(Skip = "GraphQL structure has been changed completely")]
        public void GenerateFile_OneNotBaseCurrency_CurrencySeedCodeAndCommit()
        {
            // Arrange

            var client = new GraphqlClientMock()
            {
                GraphqlQueryResponse = @"
{
    ""data"": {
        ""currencies"": [
            {
                ""id"": ""SEK"",
                ""isBaseCurrency"": false
            }
        ]
    }
}"
            };

            var sut = new LitiumMigrationGenerator(client);
            var config = ConfigurationReader.ReadConfiguration(LitiumMigrationGeneratorTests.ExampleConfiguration)[0];

            // Act
            var res = sut.GenerateFile(config);

            // Assert
            Assert.Contains("CurrencySeed.Ensure(\"SEK\")", res.Content);
            Assert.Contains("\t.IsBaseCurrency(false)", res.Content);
            Assert.Contains("\t.Commit();", res.Content);

        }
    }
}
