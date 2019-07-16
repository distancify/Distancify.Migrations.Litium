using System.Collections.Generic;

namespace Distancify.Migrations.Litium.SeedBuilder.LitiumGraphQlModel.Products
{
    public class Products
    {
        public IEnumerable<Assortment> Assortments { get; set; }
        public IEnumerable<UnitOfMeasurement> UnitOfMeasurements { get; set; }
        public IEnumerable<Inventory> Inventories { get; set; }

        public IEnumerable<CategoryDisplayTemplate> CategoryDisplayTemplates { get; set; }
    }
}
