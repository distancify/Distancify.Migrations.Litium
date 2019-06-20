using Distancify.Migrations.Litium.Generator;
using Distancify.Migrations.Litium.IntegrationTests.Mocks;
using Xunit;

namespace Distancify.Migrations.Litium.IntegrationTests
{
    public class DomainNameTests
    {
        [Fact]
        public void GenerateFile_OneDomainName_DomainNameSeedCodeAndCommit()
        {
            // Arrange

            var client = new GraphqlClientMock()
            {
                GraphqlQueryResponse = @"
{
    ""data"": {
        ""domainNames"": [
            {
                ""id"": ""distancify.com""
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
            Assert.Contains("DomainNameSeed.Ensure(\"distancify.com\")", res.Content);
            Assert.DoesNotContain(".WithRobots(", res.Content);
            Assert.DoesNotContain(".WithHttpStrictTransportSecurityMaxAge(", res.Content);
            Assert.Contains(".Commit();", res.Content);

        }

        [Fact]
        public void GenerateFile_OneDomainNameWithRobots_DomainNameSeedCodeAndCommit()
        {
            // Arrange

            var client = new GraphqlClientMock()
            {
                GraphqlQueryResponse = @"
{
    ""data"": {
        ""domainNames"": [
            {
                ""id"": ""distancify.com"",
                ""robots"": ""Robot2""
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
            Assert.Contains("DomainNameSeed.Ensure(\"distancify.com\")", res.Content);
            Assert.Contains(".WithRobots(\"Robot2\")", res.Content);
            Assert.DoesNotContain(".WithHttpStrictTransportSecurityMaxAge(", res.Content);
            Assert.Contains(".Commit();", res.Content);

        }

        [Fact]
        public void GenerateFile_OneDomainNameWithRobotsAndMaxAge_DomainNameSeedCodeAndCommit()
        {
            // Arrange

            var client = new GraphqlClientMock()
            {
                GraphqlQueryResponse = @"
{
    ""data"": {
        ""domainNames"": [
            {
                ""id"": ""distancify.com"",
                ""robots"": ""Robot2"",
                ""httpStrictTransportSecurityMaxAge"": 98765
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
            Assert.Contains("DomainNameSeed.Ensure(\"distancify.com\")", res.Content);
            Assert.Contains(".WithRobots(\"Robot2\")", res.Content);
            Assert.Contains(".WithHttpStrictTransportSecurityMaxAge(98765)", res.Content);
            Assert.Contains(".Commit();", res.Content);

        }
    }
}
