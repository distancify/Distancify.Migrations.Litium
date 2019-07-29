using Distancify.Migrations.Litium.SeedBuilder.LitiumGraphQlModel.Blocks;
using System;
using System.Collections.Generic;

namespace Distancify.Migrations.Litium.SeedBuilder.LitiumGraphQlModel.Websites
{
    public class Page : GraphQlObject
    {
        public override string Id => SystemId.ToString();

        public short Status { get; set; }

        public Guid SystemId { get; set; }
        public Guid ParentPageSystemId { get; set; }
        public Guid WebsiteSystemId { get; set; }

        public List<PageToChannelLink> ChannelLinks { get; set; }
        public List<FieldLocalization> Localizations { get; set; }
        public PageFieldTemplate FieldTemplate { get; set; }

        public List<BlockContainer> BlockContainers { get; set; }

        public List<AccessControlEntry> AccessControlList { get; set; }

        public Dictionary<string, Field> Fields { get; set; }
    }
}
