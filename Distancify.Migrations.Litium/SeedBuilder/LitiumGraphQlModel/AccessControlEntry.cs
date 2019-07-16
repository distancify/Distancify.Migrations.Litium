using Distancify.Migrations.Litium.SeedBuilder.LitiumGraphQlModel.Customers;

namespace Distancify.Migrations.Litium.SeedBuilder.LitiumGraphQlModel
{
    public class AccessControlEntry
    {
        public StaticGroup Group { get; set; }
        public string Operation { get; set; }
    }
}
