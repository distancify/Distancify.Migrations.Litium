using Distancify.Migrations.Litium.SeedBuilder.LitiumGraphQlModel.Globalization;
using System;

namespace Distancify.Migrations.Litium.SeedBuilder.LitiumGraphQlModel.Websites
{
    public class PageToChannelLink
    {
        public Guid ChannelSystemId { get; set; }

        public Channel Channel { get; set; }
    }
}
