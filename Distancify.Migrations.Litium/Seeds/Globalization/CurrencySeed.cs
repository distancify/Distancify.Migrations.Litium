using Litium;
using Litium.Globalization;
using System;
using System.Text;

namespace Distancify.Migrations.Litium.Seeds.Globalization
{
    public class CurrencySeed : ISeed, ISeedGenerator<SeedBuilder.LitiumGraphQlModel.Currency>
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

        internal static CurrencySeed CreateFrom(SeedBuilder.LitiumGraphQlModel.Currency currency)
        {
            var seed = new CurrencySeed(new Currency(currency.Id));
            return (CurrencySeed)seed.Update(currency);
        }

        public CurrencySeed IsBaseCurrency(bool on)
        {
            currency.IsBaseCurrency = on;
            return this;
        }

        public void WriteMigration(StringBuilder builder)
        {
            if (currency == null || string.IsNullOrEmpty(currency.Id))
            {
                throw new NullReferenceException("At least one Currency with an ID obtained from the GraphQL endpoint is needed in order to ensure the Currencies");
            }

            builder.AppendLine($"\t\t\t{nameof(CurrencySeed)}.{nameof(CurrencySeed.Ensure)}(\"{currency.Id}\")");
            builder.AppendLine($"\t\t\t\t.{nameof(CurrencySeed.IsBaseCurrency)}({currency.IsBaseCurrency.ToString().ToLower()})");

            builder.AppendLine("\t\t\t\t.Commit();");
        }

        public ISeedGenerator<SeedBuilder.LitiumGraphQlModel.Currency> Update(SeedBuilder.LitiumGraphQlModel.Currency data)
        {
            if (data.IsBaseCurrency.HasValue)
            {
                this.currency.IsBaseCurrency = data.IsBaseCurrency.Value;
            }
            return this;
        }



        //TODO: EchangeRate
        //TODO: GroupSeperator
        //TODO:  Symbol
        //TODO:  SymbolPosition
        //TODO: TextFormat

    }
}