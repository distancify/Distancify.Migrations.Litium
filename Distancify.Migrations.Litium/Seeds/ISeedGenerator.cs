using Distancify.Migrations.Litium.SeedBuilder.LitiumGraphqlModel;
using System.Text;

namespace Distancify.Migrations.Litium.Seeds
{
    public interface ISeedGenerator<TData>
        where TData : GraphQlObject
    {

        ISeedGenerator<TData> Update(TData data);
        void WriteMigration(StringBuilder builder);
    }
}
