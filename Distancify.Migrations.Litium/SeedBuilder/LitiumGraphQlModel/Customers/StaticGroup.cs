using System.Collections.Generic;

namespace Distancify.Migrations.Litium.SeedBuilder.LitiumGraphQlModel.Customers
{
    public class StaticGroup : GraphQlObject
    {
        public string Name { get; set; }

        public GroupFieldTemplate FieldTemplate { get; set; }

        public List<AccessControlOperationEntry> AccessControlOperationList { get; set; }
    }
}
