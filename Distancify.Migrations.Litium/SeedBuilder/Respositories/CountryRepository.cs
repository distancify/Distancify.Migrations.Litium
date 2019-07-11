using Distancify.Migrations.Litium.SeedBuilder.LitiumGraphQlModel;
using System;
using Distancify.Migrations.Litium.Seeds.Globalization;
using Distancify.Migrations.Litium.SeedBuilder.LitiumGraphQlModel.Globalization;

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
