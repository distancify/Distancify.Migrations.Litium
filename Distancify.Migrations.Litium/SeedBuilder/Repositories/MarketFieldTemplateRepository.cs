using Distancify.Migrations.Litium.SeedBuilder.LitiumGraphQlModel.Globalization;
using Distancify.Migrations.Litium.Seeds.Globalization;

namespace Distancify.Migrations.Litium.SeedBuilder.Repositories
{
    public class MarketFieldTemplateRepository : Repository<MarketFieldTemplate, MarketFieldTemplateSeed>
    {
        protected override MarketFieldTemplateSeed CreateFrom(MarketFieldTemplate graphQlItem)
        {
            return MarketFieldTemplateSeed.CreateFrom(graphQlItem);
        }
    }
}
