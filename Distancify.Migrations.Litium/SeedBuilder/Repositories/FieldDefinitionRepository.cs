using Distancify.Migrations.Litium.SeedBuilder.LitiumGraphQlModel;
using Distancify.Migrations.Litium.SeedBuilder.LitiumGraphQlModel.FieldFramework;
using Distancify.Migrations.Litium.Seeds.FieldFramework;
using Distancify.Migrations.Litium.Seeds.Globalization;

namespace Distancify.Migrations.Litium.SeedBuilder.Repositories
{
    public class FieldDefinitionRepository : Repository<FieldDefinition, FieldDefinitionSeed>
    {
        protected override FieldDefinitionSeed CreateFrom(FieldDefinition graphQlItem)
        {
            return FieldDefinitionSeed.CreateFrom(graphQlItem);
        }
    }
}
