using Distancify.Migrations.Litium.Generator;
using Distancify.Migrations.Litium.IntegrationTests.Mocks;
using Xunit;

namespace Distancify.Migrations.Litium.IntegrationTests
{
    public class LitiumMigrationGeneratorTests
    {
        [Fact]
        public void GenerateAllFiles_EmptyDataResponse_NoFiles()
        {
            // Arrange

            var client = new GraphqlClientMock()
            {
                GraphqlQueryResponse = @"{""data"": {}}"
            };

            var sut = new LitiumMigrationGenerator(client);
            var configs = sut.ReadConfiguration(ExampleConfiguration);

            // Act
            var res = sut.GenerateAllFiles(configs);

            // Assert
            Assert.Empty(res);

        }

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
            var config = sut.ReadConfiguration(ExampleConfiguration)[0];

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
            var config = sut.ReadConfiguration(ExampleConfiguration)[0];

            // Act
            var res = sut.GenerateFile(config);

            // Assert
            Assert.Contains("ChannelSeed.Ensure(\"sweden\", \"channelTemplate1\")", res.Content);
            Assert.Contains(".Commit();", res.Content);

        }

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
            var config = sut.ReadConfiguration(ExampleConfiguration)[0];

            // Act
            var res = sut.GenerateFile(config);

            // Assert
            Assert.Contains("DomainNameSeed.Ensure(\"distancify.com\")", res.Content);
            Assert.Contains(".Commit();", res.Content);

        }

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
            var config = sut.ReadConfiguration(ExampleConfiguration)[0];

            // Act
            var res = sut.GenerateFile(config);

            // Assert
            Assert.Contains("CurrencySeed.Ensure(\"SEK\")", res.Content);
            Assert.Contains(".Commit();", res.Content);

        }

        [Fact]
        public void GenerateFile_OneCountryWithCurrency_CountrySeedCodeAndCommit()
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
            var config = sut.ReadConfiguration(ExampleConfiguration)[0];

            // Act
            var res = sut.GenerateFile(config);

            // Assert
            Assert.Contains("CountrySeed.Ensure(\"SE\",\"SEK\")", res.Content);
            Assert.Contains(".Commit();", res.Content);

        }

        [Fact]
        public void GenerateFile_OneAssortment_AssortmentSeedCodeAndCommit()
        {
            // Arrange

            var client = new GraphqlClientMock()
            {
                GraphqlQueryResponse = @"
{
    ""data"": {
        ""assortments"": [
            {
                ""id"": ""assortment1""
            }
        ]
    }
}"
            };

            var sut = new LitiumMigrationGenerator(client);
            var config = sut.ReadConfiguration(ExampleConfiguration)[0];

            // Act
            var res = sut.GenerateFile(config);

            // Assert
            Assert.Contains("AssortmentSeed.Ensure(\"assortment1\")", res.Content);
            Assert.Contains(".Commit();", res.Content);

        }

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
            var config = sut.ReadConfiguration(ExampleConfiguration)[0];

            // Act
            var res = sut.GenerateFile(config);

            // Assert
            Assert.Contains("WebsiteSeed.Ensure(\"website1\",\"website1template\")", res.Content);
            Assert.Contains(".Commit();", res.Content);

        }

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
            var config = sut.ReadConfiguration(ExampleConfiguration)[0];

            // Act
            var res = sut.GenerateFile(config);

            // Assert
            Assert.Contains("LanguageSeed.Ensure(\"sv-SE\")", res.Content);
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
            var config = sut.ReadConfiguration(ExampleConfiguration)[0];

            // Act
            var res = sut.GenerateFile(config);

            // Assert
            Assert.Contains("LanguageSeed.Ensure(\"da-DK\")", res.Content);
            Assert.Contains("LanguageSeed.Ensure(\"en-GB\")", res.Content);
            Assert.Contains("LanguageSeed.Ensure(\"sv-SE\")", res.Content);
            Assert.Contains(".Commit();", res.Content);
        }


        private const string ExampleConfiguration = @"---
            - id: Migration1
              baseMigration: DevelopmentMigration
              className: TestMigration1
              host: http://localhost:56666
              namespace: Eqquo.Litium.Migrations.Production.Development
              output: c:\temp\migration\test1.cs
              query: |
                  query{
                      channels{
                          id,
                          countries{
                              id,
                              currencies{
                                  id
                              }
                          }
                      }
                  }
            - id: Migration2
              baseMigration: DevelopmentMigration
              className: TestMigration2
              host: http://localhost:56666
              namespace: Eqquo.Litium.Migrations.Production.Development
              output: c:\temp\migration\test2.cs
              query: |
                  query{
                      channels{
                          id
                      }
                  }
...";
    }
}
