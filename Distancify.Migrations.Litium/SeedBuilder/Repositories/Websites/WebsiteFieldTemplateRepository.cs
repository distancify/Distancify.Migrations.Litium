using Distancify.Migrations.Litium.SeedBuilder.LitiumGraphQlModel.Websites;
using Distancify.Migrations.Litium.Seeds.Websites;

namespace Distancify.Migrations.Litium.SeedBuilder.Repositories.Websites
{
    class WebsiteFieldTemplateRepository : Repository<WebsiteFieldTemplate, WebsiteFieldTemplateSeed>
    {
        protected override WebsiteFieldTemplateSeed CreateFrom(WebsiteFieldTemplate graphQlItem)
        {
            return WebsiteFieldTemplateSeed.CreateFrom(graphQlItem);
        }
    }
}
