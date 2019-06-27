using Distancify.Migrations.Litium.SeedBuilder.LitiumGraphqlModel;
using System.Threading.Tasks;

namespace Distancify.Migrations.Litium.SeedBuilder
{
    public interface IGraphqlClient
    {
        Task<ResponseContainer> FetchFromGraphql(MigrationConfiguration config);
    }
}
