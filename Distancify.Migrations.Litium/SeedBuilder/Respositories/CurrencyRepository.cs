using Distancify.Migrations.Litium.Seeds.Globalization;
using Distancify.Migrations.Litium.SeedBuilder.LitiumGraphQlModel.Globalization;

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
