using Distancify.Migrations.Litium.SeedBuilder.LitiumGraphQlModel;
using Distancify.Migrations.Litium.Seeds.Website;
using System.Text;

namespace Distancify.Migrations.Litium.SeedBuilder.Respositories
{
    public class WebsiteRepository : Repository<Website, WebsiteSeed>
    {
        protected override WebsiteSeed CreateFrom(Website graphQlItem)
        {
            return WebsiteSeed.CreateFrom(graphQlItem);
        }
    }
}
