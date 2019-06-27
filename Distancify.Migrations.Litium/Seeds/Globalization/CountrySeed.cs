using Litium;
using Litium.Globalization;
using System;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Distancify.Migrations.Litium.Seeds.Globalization
{
    public class CountrySeed : ISeed, ISeedGenerator<SeedBuilder.LitiumGraphqlModel.Country>
    {
        private readonly Country country;
        private string currencyId;

        private CountrySeed(Country country, string currencyId)
        {
            this.country = country;
            this.currencyId = currencyId;
        }

        public void Commit()
        {
            var countryService = IoC.Resolve<CountryService>();

            var currencySystemId = IoC.Resolve<CurrencyService>().Get(currencyId).SystemId;
            country.CurrencySystemId = currencySystemId;

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
            
            var country = IoC.Resolve<CountryService>().Get(id)?.MakeWritableClone() ??
                new Country(Guid.Empty)
                {
                    Id = id,
                    SystemId = Guid.Empty
                };

            return new CountrySeed(country, currencyId);
        }

        internal static CountrySeed CreateFrom(SeedBuilder.LitiumGraphqlModel.Country country)
        {
            var seed = new CountrySeed(new Country(Guid.Empty), string.Empty);
            return (CountrySeed)seed.Update(country);
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

        public ISeedGenerator<SeedBuilder.LitiumGraphqlModel.Country> Update(SeedBuilder.LitiumGraphqlModel.Country data)
        {
            this.country.Id = data.Id;
            if (data.Currency != null)
            {
                this.currencyId = data.Currency.Id;
            }
            if (data.StandardVatRate.HasValue)
            {
                this.country.StandardVatRate = data.StandardVatRate.Value;
            }
            return this;
        }

        public void WriteMigration(StringBuilder builder)
        {
            if (country == null || string.IsNullOrEmpty(country.Id))
            {
                throw new NullReferenceException("At least one Country with an ID obtained from the GraphQL endpoint is needed in order to ensure the Countries");
            }

            if (string.IsNullOrEmpty(currencyId))
            {
                throw new NullReferenceException("Can't ensure country if no Currency is returned from GraphQL endpoint");
            }


            builder.AppendLine($"\t\t\t{nameof(CountrySeed)}.{nameof(CountrySeed.Ensure)}(\"{country.Id}\",\"{currencyId}\")");
            if (country.StandardVatRate != 0)
            {
                builder.AppendLine($"\t\t\t\t.{nameof(CountrySeed.WithStandardVatRate)}({country.StandardVatRate.ToString(CultureInfo.InvariantCulture)})");
            }
            //WithTaxClassLink

            builder.AppendLine("\t\t\t\t.Commit();");
        }


        //TODO: Inventories
        //TODO: Price lists
    }
}