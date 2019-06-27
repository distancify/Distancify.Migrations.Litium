using Distancify.Migrations.Litium.SeedBuilder.LitiumGraphqlModel;
using System;
using Distancify.Migrations.Litium.Seeds.Globalization;

namespace Distancify.Migrations.Litium.SeedBuilder.Respositories
{
    public class CountryRepository : Repository<Country, CountrySeed>
    {

        protected override CountrySeed CreateFrom(Country graphQlItem)
        {
            return CountrySeed.CreateFrom(graphQlItem);
        }
    }
}
