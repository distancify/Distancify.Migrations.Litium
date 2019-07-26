using Distancify.Migrations.Litium.SeedBuilder.LitiumGraphQlModel.Common;
using System;
using System.Collections.Generic;

namespace Distancify.Migrations.Litium.SeedBuilder.LitiumGraphQlModel.Blocks
{
    public class Block : GraphQlObject
    {
        public override string Id => SystemId.ToString();

        public Guid SystemId { get; set; }
        public Guid FieldTemplateSystemId { get; set; }

        public short Status { get; set; }
        public bool Global { get; set; }

        public List<BlockToChannelLink> ChannelLinks { get; set; }

        public BlockFieldTemplate FieldTemplate { get; set; }

        public Dictionary<string, Field> Fields { get; set; }
    }
}
