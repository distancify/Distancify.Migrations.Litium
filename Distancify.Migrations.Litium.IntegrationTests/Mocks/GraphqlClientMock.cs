using System.Threading.Tasks;
using Distancify.Migrations.Litium.SeedBuilder;
using Distancify.Migrations.Litium.SeedBuilder.LitiumGraphQlModel;
using Newtonsoft.Json;

namespace Distancify.Migrations.Litium.IntegrationTests.Mocks
{
    public class GraphqlClientMock : IGraphqlClient
    {
        public string GraphqlQueryResponse { get; set; }

        Task<ResponseContainer> IGraphqlClient.FetchFromGraphql(MigrationConfiguration config)
        {
            return Task.FromResult(JsonConvert.DeserializeObject<ResponseContainer>(GraphqlQueryResponse));
        }
    }
}
