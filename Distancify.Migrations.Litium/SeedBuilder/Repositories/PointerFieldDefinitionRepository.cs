using Distancify.Migrations.Litium.SeedBuilder.LitiumGraphQlModel;
using Distancify.Migrations.Litium.SeedBuilder.LitiumGraphQlModel.FieldFramework.Definitions;
using Distancify.Migrations.Litium.Seeds.FieldFramework;

namespace Distancify.Migrations.Litium.SeedBuilder.Repositories
{
    public class PointerFieldDefinitionRepository : Repository<PointerFieldDefinition, PointerFieldDefinitionSeed>
    {
        protected override PointerFieldDefinitionSeed CreateFrom(PointerFieldDefinition graphQlItem)
        {
            return PointerFieldDefinitionSeed.CreateFrom(graphQlItem);
        }
    }
}
