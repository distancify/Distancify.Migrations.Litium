using Distancify.Migrations.Litium.LitiumGraphqlModel;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Distancify.Migrations.Litium.Generator
{
    public class GraphqlClient : IGraphqlClient
    {
        public ResponseContainer FetchFromGraphql(MigrationConfiguration config)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var response = client.PostAsync(config.Host + "/graphql", new StringContent(config.Query)).Result;

                return JsonConvert.DeserializeObject<ResponseContainer>(response.Content.ToString());
            }
        }
    }
}

