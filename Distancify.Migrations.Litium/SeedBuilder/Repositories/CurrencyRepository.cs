﻿using Distancify.Migrations.Litium.SeedBuilder.LitiumGraphQlModel.Globalization;
using Distancify.Migrations.Litium.Seeds.Globalization;

namespace Distancify.Migrations.Litium.SeedBuilder.Repositories
{
    public class CurrencyRepository : Repository<Currency, CurrencySeed>
    {
        protected override CurrencySeed CreateFrom(Currency graphQlItem)
        {
            return CurrencySeed.CreateFrom(graphQlItem);
        }
    }
}
