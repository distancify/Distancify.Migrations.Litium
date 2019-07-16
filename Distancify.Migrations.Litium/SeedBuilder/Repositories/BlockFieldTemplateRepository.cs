using Distancify.Migrations.Litium.SeedBuilder.LitiumGraphQlModel.Blocks;
using Distancify.Migrations.Litium.Seeds.Websites;

namespace Distancify.Migrations.Litium.SeedBuilder.Repositories
{
    public class BlockFieldTemplateRepository : Repository<BlockFieldTemplate, BlockFieldTemplateSeed>
    {
        protected override BlockFieldTemplateSeed CreateFrom(BlockFieldTemplate graphQlItem)
        {
            return BlockFieldTemplateSeed.CreateFrom(graphQlItem);
        }
    }
}
