using System.Collections.Generic;

namespace Distancify.Migrations.Litium.SeedBuilder.LitiumGraphqlModel
{
    public class Channel : GraphQlObject
    {
        public ChannelFieldTemplate FieldTemplate { get; set; }
        public IEnumerable<ChannelDomainProperties> Domains { get; set; }

        public IEnumerable<Country> Countries { get; set; }
    }
}
