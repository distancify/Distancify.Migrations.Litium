using System.Collections.Generic;

namespace Distancify.Migrations.Litium.SeedBuilder.LitiumGraphQlModel
{
    public class Data
    {
        public Common.Common Common { get; set; }
        public Globalization.Globalization Globalization { get; set; }
        public Blocks.Blocks Blocks { get; set; }
        public Products.Products Products { get; set; }
        public Websites.RootWebsite Websites { get; set; }
        public Customers.Customers Customers { get; set; }
    }
}
