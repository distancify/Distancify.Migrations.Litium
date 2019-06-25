using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distancify.Migrations.Litium.LitiumGraphqlModel
{
    public class Country : GraphQlObject
    {
        public Guid SystemId { get; set; }

        public Currency Currency { get; set; }

        //taxclass
        public double? StandardVatRate { get; set; }
    }
}
