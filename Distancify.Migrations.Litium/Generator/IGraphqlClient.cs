using Distancify.Migrations.Litium.LitiumGraphqlModel;

namespace Distancify.Migrations.Litium.Generator
{
    public interface IGraphqlClient
    {
        ResponseContainer FetchFromGraphql(MigrationConfiguration config);
    }
}
