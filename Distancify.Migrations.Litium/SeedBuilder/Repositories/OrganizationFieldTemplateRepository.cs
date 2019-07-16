using Distancify.Migrations.Litium.SeedBuilder.LitiumGraphQlModel.Customers;
using Distancify.Migrations.Litium.Seeds.Customer;

namespace Distancify.Migrations.Litium.SeedBuilder.Repositories
{
    public class OrganizationFieldTemplateRepository : Repository<OrganizationFieldTemplate, OrganizationFieldTemplateSeed>
    {
        protected override OrganizationFieldTemplateSeed CreateFrom(OrganizationFieldTemplate graphQlItem)
        {
            return OrganizationFieldTemplateSeed.CreateFrom(graphQlItem);
        }
    }
}
