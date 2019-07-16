using Distancify.Migrations.Litium.SeedBuilder.LitiumGraphQlModel.Customers;
using Distancify.Migrations.Litium.Seeds.Customer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distancify.Migrations.Litium.SeedBuilder.Repositories
{
    public class PersonFieldTemplateRepository : Repository<PersonFieldTemplate, PersonFieldTemplateSeed>
    {
        protected override PersonFieldTemplateSeed CreateFrom(PersonFieldTemplate graphQlItem)
        {
            return PersonFieldTemplateSeed.CreateFrom(graphQlItem);
        }
    }
}
