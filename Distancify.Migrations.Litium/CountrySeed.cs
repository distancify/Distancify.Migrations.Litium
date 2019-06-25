using Litium;
using Litium.Globalization;
using System;

namespace Distancify.Migrations.Litium
{
    public class CountrySeed : ISeed
    {
        public const string Sweden = "SE";
        public const string UnitedKingdom = "GB";
        public const string Norway = "NO";
        public const string Finland = "FI";
        public const string EU = "EU";
        public const string Germany = "DE";
        public const string Netherlands = "NL";
        public const string World = "001";

        private readonly Country Country;

        private CountrySeed(Country country)
        {
            this.Country = country;
        }

        public void Commit()
        {
            var countryService = IoC.Resolve<CountryService>();

            if (Country.SystemId == Guid.Empty)
            {
                Country.SystemId = Guid.NewGuid();
                countryService.Create(Country);
            }
            else
            {
                countryService.Update(Country);
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
            Country.StandardVatRate = standardVatRate;
            return this;
        }
    }
}