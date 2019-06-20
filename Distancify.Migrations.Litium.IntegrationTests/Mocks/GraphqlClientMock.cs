using Distancify.Migrations.Litium.Generator;
using Distancify.Migrations.Litium.LitiumGraphqlModel;
using Newtonsoft.Json;

namespace Distancify.Migrations.Litium.IntegrationTests.Mocks
{
    public class GraphqlClientMock : IGraphqlClient
    {
        public string GraphqlQueryResponse { get; set; }
        public ResponseContainer FetchFromGraphql(MigrationConfiguration config)
        {
            return JsonConvert.DeserializeObject<ResponseContainer>(GraphqlQueryResponse);
        }
    }
}
