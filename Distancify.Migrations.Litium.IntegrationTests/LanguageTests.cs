using Distancify.Migrations.Litium.Generator;
using Distancify.Migrations.Litium.IntegrationTests.Mocks;
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
            var config = sut.ReadConfiguration(LitiumMigrationGeneratorTests.ExampleConfiguration)[0];

            // Act
            var res = sut.GenerateFile(config);

            // Assert
            Assert.Contains("LanguageSeed.Ensure(\"sv-SE\")", res.Content);
            Assert.DoesNotContain(".IsDefaultLanguage(", res.Content);
            Assert.Contains(".Commit();", res.Content);

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
            var config = sut.ReadConfiguration(LitiumMigrationGeneratorTests.ExampleConfiguration)[0];

            // Act
            var res = sut.GenerateFile(config);

            // Assert
            Assert.Contains("LanguageSeed.Ensure(\"sv-SE\")", res.Content);
            Assert.Contains(".IsDefaultLanguage(true)", res.Content);
            Assert.Contains(".Commit();", res.Content);

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
            var config = sut.ReadConfiguration(LitiumMigrationGeneratorTests.ExampleConfiguration)[0];

            // Act
            var res = sut.GenerateFile(config);

            // Assert
            Assert.Contains("LanguageSeed.Ensure(\"sv-SE\")", res.Content);
            Assert.Contains(".IsDefaultLanguage(false)", res.Content);
            Assert.Contains(".Commit();", res.Content);

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
            var config = sut.ReadConfiguration(LitiumMigrationGeneratorTests.ExampleConfiguration)[0];

            // Act
            var res = sut.GenerateFile(config);

            // Assert
            Assert.Contains("LanguageSeed.Ensure(\"da-DK\")", res.Content);
            Assert.Contains("LanguageSeed.Ensure(\"en-GB\")", res.Content);
            Assert.Contains("LanguageSeed.Ensure(\"sv-SE\")", res.Content);
            Assert.Contains(".Commit();", res.Content);
        }
    }
}
