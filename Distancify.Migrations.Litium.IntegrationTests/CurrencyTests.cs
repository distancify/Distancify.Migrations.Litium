using Distancify.Migrations.Litium.Generator;
using Distancify.Migrations.Litium.IntegrationTests.Mocks;
using Xunit;

namespace Distancify.Migrations.Litium.IntegrationTests
{
    public class CurrencyTests
    {
        [Fact]
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
            var config = sut.ReadConfiguration(LitiumMigrationGeneratorTests.ExampleConfiguration)[0];

            // Act
            var res = sut.GenerateFile(config);

            // Assert
            Assert.Contains("CurrencySeed.Ensure(\"SEK\")", res.Content);
            Assert.DoesNotContain("CurrencySeed.IsBaseCurrency(", res.Content);
            Assert.Contains(".Commit();", res.Content);

        }

        [Fact]
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
            var config = sut.ReadConfiguration(LitiumMigrationGeneratorTests.ExampleConfiguration)[0];

            // Act
            var res = sut.GenerateFile(config);

            // Assert
            Assert.Contains("CurrencySeed.Ensure(\"SEK\")", res.Content);
            Assert.Contains("CurrencySeed.IsBaseCurrency(true)", res.Content);
            Assert.Contains(".Commit();", res.Content);

        }

        [Fact]
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
            var config = sut.ReadConfiguration(LitiumMigrationGeneratorTests.ExampleConfiguration)[0];

            // Act
            var res = sut.GenerateFile(config);

            // Assert
            Assert.Contains("CurrencySeed.Ensure(\"SEK\")", res.Content);
            Assert.Contains("CurrencySeed.IsBaseCurrency(false)", res.Content);
            Assert.Contains(".Commit();", res.Content);

        }
    }
}
