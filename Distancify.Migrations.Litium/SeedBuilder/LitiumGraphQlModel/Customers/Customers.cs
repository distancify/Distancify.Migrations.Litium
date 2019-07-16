using System.Collections.Generic;

namespace Distancify.Migrations.Litium.SeedBuilder.LitiumGraphQlModel.Customers
{
    public class Customers
    {
        public IEnumerable<PersonFieldTemplate> PersonFieldTemplates { get; set; }
        public IEnumerable<OrganizationFieldTemplate> OrganizationFieldTemplates { get; set; }
        public IEnumerable<GroupFieldTemplate> GroupFieldTemplates { get; set; }
    }
}
