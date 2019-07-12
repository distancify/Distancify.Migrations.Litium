using Distancify.Migrations.Litium.SeedBuilder.LitiumGraphQlModel.Globalization;
using Distancify.Migrations.Litium.Seeds.Globalization;


namespace Distancify.Migrations.Litium.SeedBuilder.Repositories
{
    public class MarketRepository : Repository<Market, MarketSeed>
    {
        protected override MarketSeed CreateFrom(Market graphQlItem)
        {
            return MarketSeed.CreateFrom(graphQlItem);
        }
    }
}
