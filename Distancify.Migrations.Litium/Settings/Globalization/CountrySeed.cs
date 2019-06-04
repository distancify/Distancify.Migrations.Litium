using Litium;
using Litium.Globalization;
using System;
using System.Linq;

namespace Distancify.Migrations.Litium.Settings.Globalization
{
    public class CountrySeed : ISeed
    {

        private readonly Country country;

        private CountrySeed(Country country)
        {
            this.country = country;
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

        //TODO: Taxclass links
        //TODO: Inventories
        //TODO: Price lists
    }
}