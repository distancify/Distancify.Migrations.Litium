using Distancify.Migrations.Litium.IntegrationTests.Mocks;
using Distancify.Migrations.Litium.SeedBuilder;
using Xunit;

namespace Distancify.Migrations.Litium.IntegrationTests
{
    public class LanguageTests
    {
        [Fact]
        public void GenerateFile_OneLanguage_LanguageSeedCodeAndCommit()
        {
            // Arrange

            var client = new GraphqlClientMock()
            {
                GraphqlQueryResponse = @"
{
    ""data"": {
        ""languages"": [
            {
                ""id"": ""sv-SE""
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
            Assert.Contains("LanguageSeed.Ensure(\"sv-SE\")", res.Content);
            Assert.Contains("\t.Commit();", res.Content);

        }

        [Fact]
        public void GenerateFile_OneLanguageWhichIsDefault_LanguageSeedCodeAndCommit()
        {
            // Arrange

            var client = new GraphqlClientMock()
            {
                GraphqlQueryResponse = @"
{
    ""data"": {
        ""languages"": [
            {
                ""id"": ""sv-SE"",
                ""isDefaultLanguage"": true
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
            Assert.Contains("LanguageSeed.Ensure(\"sv-SE\")", res.Content);
            Assert.Contains("\t.IsDefaultLanguage(true)", res.Content);
            Assert.Contains("\t.Commit();", res.Content);

        }

        [Fact]
        public void GenerateFile_OneLanguageWhichIsNotDefault_LanguageSeedCodeAndCommit()
        {
            // Arrange

            var client = new GraphqlClientMock()
            {
                GraphqlQueryResponse = @"
{
    ""data"": {
        ""languages"": [
            {
                ""id"": ""sv-SE"",
                ""isDefaultLanguage"": false
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
            Assert.Contains("LanguageSeed.Ensure(\"sv-SE\")", res.Content);
            Assert.Contains("\t.IsDefaultLanguage(false)", res.Content);
            Assert.Contains("\t.Commit();", res.Content);

        }

        [Fact]
        public void GenerateFile_ThreeLanguages_ThreeLanguageSeedCodeAndCommit()
        {
            // Arrange

            var client = new GraphqlClientMock()
            {
                GraphqlQueryResponse = @"
        {
    ""data"": {
        ""languages"": [
            {
                ""id"": ""da-DK""
            },
            {
                ""id"": ""en-GB""
            },
            {
                ""id"": ""sv-SE""
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
            Assert.Contains("LanguageSeed.Ensure(\"da-DK\")", res.Content);
            Assert.Contains("LanguageSeed.Ensure(\"en-GB\")", res.Content);
            Assert.Contains("LanguageSeed.Ensure(\"sv-SE\")", res.Content);
            Assert.Contains("\t.Commit();", res.Content);
        }
    }
}
