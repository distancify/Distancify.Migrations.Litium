using Distancify.Migrations.Litium.SeedBuilder.LitiumGraphQlModel.Globalization;
using System;

namespace Distancify.Migrations.Litium.SeedBuilder.LitiumGraphQlModel.Blocks
{
    public class BlockToChannelLink
    {
        public Guid ChannelSystemId { get; set; }
        public Channel Channel { get; set; }
    }
}
