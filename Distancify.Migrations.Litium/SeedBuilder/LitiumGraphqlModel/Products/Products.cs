using System.Collections.Generic;

namespace Distancify.Migrations.Litium.SeedBuilder.LitiumGraphQlModel.Products
{
    public class Products
    {
        public IEnumerable<Assortment> Assortments { get; set; }
        public IEnumerable<UnitOfMeasurement> UnitOfMeasurements { get; set; }
        public IEnumerable<Inventory> Inventories { get; set; }

        public IEnumerable<ProductFieldTemplate> ProductFieldTemplates { get; set; }
        public IEnumerable<CategoryFieldTemplate> CategoryFieldTemplates { get; set; }

        public IEnumerable<CategoryDisplayTemplate> CategoryDisplayTemplates { get; set; }
        public IEnumerable<ProductDisplayTemplate> ProductDisplayTemplates { get; set; }

    }
}
