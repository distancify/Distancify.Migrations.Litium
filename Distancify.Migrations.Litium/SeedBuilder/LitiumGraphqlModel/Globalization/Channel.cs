using System.Collections.Generic;

namespace Distancify.Migrations.Litium.SeedBuilder.LitiumGraphQlModel.Globalization
{
    public class Channel : GraphQlObject
    {
        public ChannelFieldTemplate FieldTemplate { get; set; }
        public IEnumerable<ChannelDomainProperties> Domains { get; set; }

        public IEnumerable<Country> Countries { get; set; }

    }
}
