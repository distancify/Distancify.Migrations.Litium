using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distancify.Migrations.Litium.SeedBuilder.LitiumGraphQlModel
{
    public class DecimalOptionItem
    {
        public decimal Value { get; set; }
        public List<FieldLocalization> Localizations { get; set; }
    }
}
