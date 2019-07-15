using Distancify.Migrations.Litium.SeedBuilder.LitiumGraphQlModel.Websites;
using Distancify.Migrations.Litium.Seeds.Websites;

namespace Distancify.Migrations.Litium.SeedBuilder.Repositories
{
    public class WebsiteRepository : Repository<Website, WebsiteSeed>
    {
        protected override WebsiteSeed CreateFrom(Website graphQlItem)
        {
            return WebsiteSeed.CreateFrom(graphQlItem);
        }
    }
}
