using Distancify.Migrations.Litium.SeedBuilder.LitiumGraphQlModel.Customers;
using Distancify.Migrations.Litium.Seeds.Customer;

namespace Distancify.Migrations.Litium.SeedBuilder.Repositories
{
    public class GroupFieldTemplateRepository : Repository<GroupFieldTemplate, GroupFieldTemplateSeed>
    {
        protected override GroupFieldTemplateSeed CreateFrom(GroupFieldTemplate graphQlItem)
        {
            return GroupFieldTemplateSeed.CreateFrom(graphQlItem);
        }
    }
}
