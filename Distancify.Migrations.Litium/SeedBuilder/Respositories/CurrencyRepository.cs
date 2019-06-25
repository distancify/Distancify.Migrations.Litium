using Distancify.Migrations.Litium.SeedBuilder.LitiumGraphqlModel;
using System;
using System.Text;
using Distancify.Migrations.Litium.Seeds.GlobalizationSeeds;

namespace Distancify.Migrations.Litium.SeedBuilder.Respositories
{
    public class CurrencyRepository : Repository<Currency>
    {

        public override void AppendMigration(StringBuilder builder)
        {
            foreach (var currency in Items.Values)
            {
                if (currency == null || string.IsNullOrEmpty(currency.Id))
                {
                    throw new NullReferenceException("At least one Currency with an ID obtained from the GraphQL endpoint is needed in order to ensure the Currencies");
                }

                builder.AppendLine($"\t\t\t{nameof(CurrencySeed)}.{nameof(CurrencySeed.Ensure)}(\"{currency.Id}\")");
                if (currency.IsBaseCurrency.HasValue)
                {
                    builder.AppendLine($"\t\t\t\t.{nameof(CurrencySeed.IsBaseCurrency)}({currency.IsBaseCurrency.Value.ToString().ToLower()})");
                }

                builder.AppendLine("\t\t\t\t.Commit();");
            }
        }
    }
}
