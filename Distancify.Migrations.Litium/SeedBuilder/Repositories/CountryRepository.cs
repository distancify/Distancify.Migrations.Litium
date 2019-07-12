using Distancify.Migrations.Litium.SeedBuilder.LitiumGraphQlModel.Globalization;
using Distancify.Migrations.Litium.Seeds.Globalization;

namespace Distancify.Migrations.Litium.SeedBuilder.Repositories
{
    public class CountryRepository : Repository<Country, CountrySeed>
    {
        protected override CountrySeed CreateFrom(Country graphQlItem)
        {
            return CountrySeed.CreateFrom(graphQlItem);
        }
    }
}
