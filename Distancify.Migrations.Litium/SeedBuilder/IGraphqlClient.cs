using Distancify.Migrations.Litium.SeedBuilder.LitiumGraphQlModel;
using System.Threading.Tasks;

namespace Distancify.Migrations.Litium.SeedBuilder
{
    public interface IGraphqlClient
    {
        Task<ResponseContainer> FetchFromGraphql(MigrationConfiguration config);
    }
}
