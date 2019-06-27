using Distancify.Migrations.Litium.SeedBuilder.LitiumGraphqlModel;
using System;
using Distancify.Migrations.Litium.Seeds.Globalization;

namespace Distancify.Migrations.Litium.SeedBuilder.Respositories
{
    public class CurrencyRepository : Repository<Currency, CurrencySeed>
    {

          protected override CurrencySeed CreateFrom(Currency graphQlItem)
        {
            return CurrencySeed.CreateFrom(graphQlItem);

        }
    }
}
