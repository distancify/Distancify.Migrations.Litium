using Distancify.Migrations.Litium.SeedBuilder.LitiumGraphQlModel;
using Distancify.Migrations.Litium.Seeds.Globalization;

namespace Distancify.Migrations.Litium.SeedBuilder.Respositories
{
    public class FieldDefinitionRepository : Repository<FieldDefinition, FieldDefinitionSeed>
    {
        protected override FieldDefinitionSeed CreateFrom(FieldDefinition graphQlItem)
        {
            return FieldDefinitionSeed.CreateFrom(graphQlItem);
        }
    }
}
