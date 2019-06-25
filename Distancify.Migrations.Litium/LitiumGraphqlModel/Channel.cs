using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distancify.Migrations.Litium.LitiumGraphqlModel
{
    public class Channel : GraphQlObject
    {
        public ChannelFieldTemplate FieldTemplate { get; set; }
        public IEnumerable<ChannelDomainProperties> Domains { get; set; }

        public IEnumerable<Country> Countries { get; set; }
    }
}
