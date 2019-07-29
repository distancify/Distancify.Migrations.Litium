using Distancify.Migrations.Litium.SeedBuilder.LitiumGraphQlModel;
using Distancify.Migrations.Litium.Seeds.FieldFramework;

namespace Distancify.Migrations.Litium.SeedBuilder.Repositories
{
    public class MultiFieldDefinitionRepository : Repository<MultiFieldDefinition, MultiFieldDefinitionSeed>
    {
        protected override MultiFieldDefinitionSeed CreateFrom(MultiFieldDefinition graphQlItem)
        {
            return MultiFieldDefinitionSeed.CreateFrom(graphQlItem);
        }
    }
}
