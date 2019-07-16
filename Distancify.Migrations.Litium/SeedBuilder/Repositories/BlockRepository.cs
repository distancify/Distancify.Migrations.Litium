using Distancify.Migrations.Litium.SeedBuilder.LitiumGraphQlModel.Blocks;
using Distancify.Migrations.Litium.Seeds.Websites;

namespace Distancify.Migrations.Litium.SeedBuilder.Repositories
{
    public class BlockRepository : Repository<Block, BlockSeed>
    {
        protected override BlockSeed CreateFrom(Block graphQlItem)
        {
            return BlockSeed.CreateFrom(graphQlItem);
        }
    }
}
