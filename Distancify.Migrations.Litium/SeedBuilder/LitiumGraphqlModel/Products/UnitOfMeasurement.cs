using System.Collections.Generic;

namespace Distancify.Migrations.Litium.SeedBuilder.LitiumGraphQlModel.Products
{
    public class UnitOfMeasurement : GraphQlObject
    {
        public int DecimalDigits { get; set; }
        public string SystemId { get; set; }

        public List<FieldLocalization> Localizations { get; set; }
    }
}
