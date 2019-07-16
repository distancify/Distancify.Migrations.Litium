using System.Collections.Generic;

namespace Distancify.Migrations.Litium.SeedBuilder.LitiumGraphQlModel.Blocks
{
    public class BlockContainer : GraphQlObject
    {
        public List<Block> Blocks { get; set; }
    }
}
