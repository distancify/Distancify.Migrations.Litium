using Litium;
using Litium.Globalization;
using System;
using System.Linq;
using System.Text;
using Graphql = Distancify.Migrations.Litium.LitiumGraphqlModel;

namespace Distancify.Migrations.Litium.Globalization
{
    public class CountrySeed : ISeed
    {
        private readonly Graphql.Country graphqlCountry;
        private readonly Country country;

        private CountrySeed(Country country)
        {
            this.country = country;
        }

        public CountrySeed(Graphql.Country graphqlCountry)
        {
            this.graphqlCountry = graphqlCountry;
        }

        public void Commit()
        {
            var countryService = IoC.Resolve<CountryService>();

            if (country.SystemId == Guid.Empty)
            {
                country.SystemId = Guid.NewGuid();
                countryService.Create(country);
            }
            else
            {
                countryService.Update(country);
            }
        }

        public static CountrySeed Ensure(string id, string currencyId)
        {
            var currencySystemId = IoC.Resolve<CurrencyService>().Get(currencyId).SystemId;
            var country = IoC.Resolve<CountryService>().Get(id)?.MakeWritableClone() ??
                new Country(currencySystemId)
                {
                    Id = id,
                    SystemId = Guid.Empty
                };

            return new CountrySeed(country);
        }

        public CountrySeed WithStandardVatRate(decimal standardVatRate)
        {
            country.StandardVatRate = standardVatRate;
            return this;
        }

        public CountrySeed WithTaxClassLink(string taxClassId, decimal vatRate)
        {
            var taxClassSystemGuid = IoC.Resolve<TaxClassService>().Get(taxClassId).SystemId;
            var taxClassLink = country.TaxClassLinks.FirstOrDefault(t => t.TaxClassSystemId == taxClassSystemGuid);
            if (taxClassLink == null)
            {
                country.TaxClassLinks.Add(
                new CountryToTaxClassLink(taxClassSystemGuid)
                {
                    VatRate = vatRate
                });

                return this;
            }

            taxClassLink.VatRate = vatRate;

            return this;
        }

        public string GenerateMigration()
        {
            if (graphqlCountry.Currency == null)
            {
                Console.WriteLine("Error: Can't ensure country if no Currency is returned from GraphQL endpoint");
                return string.Empty;
            }

            StringBuilder builder = new StringBuilder();

            builder.AppendLine($"\t\t\t{nameof(CountrySeed)}.{nameof(CountrySeed.Ensure)}(\"{graphqlCountry.Id}\",\"{graphqlCountry.Currency.Id}\")");


            builder.AppendLine("\t\t\t\t.Commit();");
            return builder.ToString();
        }

        //TODO: Taxclass links
        //TODO: Inventories
        //TODO: Price lists
    }
}