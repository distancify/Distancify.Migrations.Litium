using Distancify.Migrations.Litium.SeedBuilder.LitiumGraphQlModel.Websites;
using Distancify.Migrations.Litium.Seeds.Websites;

namespace Distancify.Migrations.Litium.SeedBuilder.Repositories.Websites
{
    public class PageRepository : Repository<Page, PageSeed>
    {
        protected override PageSeed CreateFrom(Page graphQlItem)
        {
            return PageSeed.CreateFrom(graphQlItem);
        }
    }
}
