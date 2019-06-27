using Distancify.Migrations.Litium.SeedBuilder.LitiumGraphqlModel;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Distancify.Migrations.Litium.SeedBuilder
{
    public class GraphqlClient : IGraphqlClient
    {
        public async Task<ResponseContainer> FetchFromGraphql(MigrationConfiguration config)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var response = await client.PostAsync(config.Host + "/graphql", new StringContent(config.Query));

                return JsonConvert.DeserializeObject<ResponseContainer>(await response.Content.ReadAsStringAsync());
            }
        }
    }
}

