using Litium;
using Litium.Globalization;
using System;

namespace Distancify.Migrations.Litium.Settings.Globalization
{
    public class CurrencySeed : ISeed
    {


        private readonly Currency currency;

        private CurrencySeed(Currency currency)
        {
            this.currency = currency;
        }

        public void Commit()
        {
            var currencyService = IoC.Resolve<CurrencyService>();

            if (currency.SystemId == Guid.Empty)
            {
                currency.SystemId = Guid.NewGuid();
                currencyService.Create(currency);
                return;
            }

            currencyService.Update(currency);
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
            currency.IsBaseCurrency = on;
            return this;
        }

        //TODO: EchangeRate
        //TODO: GroupSeperator
        //TODO:  Symbol
        //TODO:  SymbolPosition
        //TODO: TextFormat

    }
}