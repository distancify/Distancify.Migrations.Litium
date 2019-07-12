using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distancify.Migrations.Litium.SeedBuilder.LitiumGraphQlModel
{
    public class UnitOfMeasurement : GraphQlObject
    {
        public int DecimalDigits { get; set; }
        public string SystemId { get; set; }

        public List<Localization> Localizations { get; set; }
    }

    public class Localization
    {
        public string Culture { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
