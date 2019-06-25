using Distancify.Migrations.Litium.Globalization;
using Distancify.Migrations.Litium.LitiumGraphqlModel;
using System;
using System.Text;

namespace Distancify.Migrations.Litium.Generator
{
    public class CountryRepository : Repository<Country>
    {

        public override void AppendMigration(StringBuilder builder)
        {
            foreach (var country in Items.Values)
            {
                if (country == null || string.IsNullOrEmpty(country.Id))
                {
                    throw new NullReferenceException("At least one Country with an ID obtained from the GraphQL endpoint is needed in order to ensure the Countries");
                }

                if (country.Currency == null)
                {
                    throw new NullReferenceException("Can't ensure country if no Currency is returned from GraphQL endpoint");
                }


                builder.AppendLine($"\t\t\t{nameof(CountrySeed)}.{nameof(CountrySeed.Ensure)}(\"{country.Id}\",\"{country.Currency.Id}\")");
                if (country.StandardVatRate.HasValue)
                {
                    builder.AppendLine($"\t\t\t\t.{nameof(CountrySeed.WithStandardVatRate)}({country.StandardVatRate.Value})");
                }
                //WithTaxClassLink

                builder.AppendLine("\t\t\t\t.Commit();");

            }
        }
    }
}
