using Distancify.Migrations.Litium.SeedBuilder.LitiumGraphQlModel;
using Distancify.Migrations.Litium.Seeds.BaseSeeds;

namespace Distancify.Migrations.Litium.SeedBuilder.Repositories
{
    public class TextOptionFieldDefinitionsRepository : Repository<TextOptionFieldDefinition, TextOptionFieldDefinitionSeed>
    {
        protected override TextOptionFieldDefinitionSeed CreateFrom(TextOptionFieldDefinition graphQlItem)
        {
            return TextOptionFieldDefinitionSeed.CreateFrom(graphQlItem);
        }
    }
}
