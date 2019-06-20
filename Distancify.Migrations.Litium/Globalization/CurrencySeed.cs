using Litium;
using Litium.Globalization;
using System;
using System.Text;
using Graphql = Distancify.Migrations.Litium.LitiumGraphqlModel;

namespace Distancify.Migrations.Litium.Globalization
{
    public class CurrencySeed : ISeed
    {

        private Graphql.Currency graphqlCurrency;
        private readonly Currency currency;

        public CurrencySeed(Graphql.Currency graphqlCurrency)
        {
            this.graphqlCurrency = graphqlCurrency;
        }

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

        public string GenerateMigration()
        {
            if (graphqlCurrency == null || string.IsNullOrEmpty(graphqlCurrency.Id))
            {
                throw new NullReferenceException("At least one Currency with an ID obtained from the GraphQL endpoint is needed in order to ensure the Currencies");
            }

            StringBuilder builder = new StringBuilder();

            builder.AppendLine($"\t\t\t{nameof(CurrencySeed)}.{nameof(CurrencySeed.Ensure)}(\"{graphqlCurrency.Id}\")");
            if (graphqlCurrency.IsBaseCurrency.HasValue)
            {
                builder.AppendLine($"\t\t\t\t{nameof(CurrencySeed)}.{nameof(CurrencySeed.IsBaseCurrency)}({graphqlCurrency.IsBaseCurrency.Value.ToString().ToLower()})");
            }

            builder.AppendLine("\t\t\t\t.Commit();");
            return builder.ToString();
        }

        //TODO: EchangeRate
        //TODO: GroupSeperator
        //TODO:  Symbol
        //TODO:  SymbolPosition
        //TODO: TextFormat

    }
}