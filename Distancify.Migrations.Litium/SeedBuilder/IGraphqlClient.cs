using Distancify.Migrations.Litium.SeedBuilder.LitiumGraphqlModel;

namespace Distancify.Migrations.Litium.SeedBuilder
{
    public interface IGraphqlClient
    {
        ResponseContainer FetchFromGraphql(MigrationConfiguration config);
    }
}
