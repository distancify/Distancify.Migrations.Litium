using Litium;
using Litium.Globalization;
using System;

namespace Distancify.Migrations.Litium
{
    public class CurrencySeed : ISeed
    {
        public const string SwedishKrona = "SEK";
        public const string PoundSterling = "GBP";

        private readonly Currency Currency;

        private CurrencySeed(Currency currency)
        {
            this.Currency = currency;
        }

        public void Commit()
        {
            var currencyService = IoC.Resolve<CurrencyService>();

            if (Currency.SystemId == Guid.Empty)
            {
                Currency.SystemId = Guid.NewGuid();
                currencyService.Create(Currency);
            }
            else
            {
                currencyService.Update(Currency);
            }
        }

        public static CurrencySeed Ensure(string id)
        {
            var currency = IoC.Resolve<CurrencyService>().Get(id)?.MakeWritableClone() ??
                new Currency(id)
                {
                    SystemId = Guid.Empty
                };

            return new CurrencySeed(currency);
        }

        public CurrencySeed IsBaseCurrency(bool on)
        {
            Currency.IsBaseCurrency = on;
            return this;
        }
    }
}