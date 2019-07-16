using Distancify.Migrations.Litium.SeedBuilder.LitiumGraphQlModel.Websites;
using Distancify.Migrations.Litium.Seeds.Websites;

namespace Distancify.Migrations.Litium.SeedBuilder.Repositories.Websites
{
    public class PageFieldTemplateRepository : Repository<PageFieldTemplate, PageFieldTemplateSeed>
    {
        protected override PageFieldTemplateSeed CreateFrom(PageFieldTemplate graphQlItem)
        {
            return PageFieldTemplateSeed.CreateFrom(graphQlItem);
        }
    }
}
