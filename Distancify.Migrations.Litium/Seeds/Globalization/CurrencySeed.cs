using Litium;
using Litium.Globalization;
using System;
using System.Text;

namespace Distancify.Migrations.Litium.Seeds.Globalization
{
    public class CurrencySeed : ISeed, ISeedGenerator<SeedBuilder.LitiumGraphQlModel.Globalization.Currency>
    {
        private readonly Currency _currency;

        private CurrencySeed(Currency currency)
        {
           _currency = currency;
        }

        public void Commit()
        {
            var currencyService = IoC.Resolve<CurrencyService>();

            if (_currency.SystemId == Guid.Empty)
            {
                _currency.SystemId = Guid.NewGuid();
                currencyService.Create(_currency);
                return;
            }

            currencyService.Update(_currency);
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

        internal static CurrencySeed CreateFrom(SeedBuilder.LitiumGraphQlModel.Globalization.Currency currency)
        {
            var seed = new CurrencySeed(new Currency(currency.Id));
            return (CurrencySeed)seed.Update(currency);
        }

        public CurrencySeed WithSymbol(string symbol)
        {
            _currency.Symbol = symbol;
            return this;
        }

        public CurrencySeed WithSymbolPosition(Currency.Positions position)
        {
            _currency.SymbolPosition = position;
            return this;
        }

        public CurrencySeed WithExchangeRate(decimal exchangeRate)
        {
            _currency.ExchangeRate = exchangeRate;
            return this;
        }

        public CurrencySeed WithGroupSeparator(string groupSeparator)
        {
            _currency.GroupSeparator = groupSeparator;
            return this;
        }

        public CurrencySeed WithTextFormat(string textFormat)
        {
            _currency.TextFormat = textFormat;
            return this;
        }

        public CurrencySeed IsBaseCurrency(bool on)
        {
            _currency.IsBaseCurrency = on;
            return this;
        }

        public void WriteMigration(StringBuilder builder)
        {
            if (_currency == null || string.IsNullOrEmpty(_currency.Id))
            {
                throw new NullReferenceException("At least one Currency with an ID obtained from the GraphQL endpoint is needed in order to ensure the Currencies");
            }

            builder.AppendLine($"\r\n\t\t\t{nameof(CurrencySeed)}.{nameof(Ensure)}(\"{_currency.Id}\")");
            builder.AppendLine($"\t\t\t\t.{nameof(IsBaseCurrency)}({_currency.IsBaseCurrency.ToString().ToLower()})");

            if (!string.IsNullOrEmpty(_currency.Symbol))
            {
                builder.AppendLine($"\t\t\t\t.{nameof(WithSymbol)}(\"{_currency.Symbol}\")");
                builder.AppendLine($"\t\t\t\t.{nameof(WithSymbolPosition)}({_currency.SymbolPosition})");
            }

            if (_currency.ExchangeRate != 0)
            {
                builder.AppendLine($"\t\t\t\t.{nameof(WithExchangeRate)}({_currency.ExchangeRate})");
            }

            if (!string.IsNullOrEmpty(_currency.GroupSeparator))
            {
                builder.AppendLine($"\t\t\t\t.{nameof(WithGroupSeparator)}(\"{_currency.GroupSeparator}\")");
            }

            if (!string.IsNullOrEmpty(_currency.TextFormat))
            {
                builder.AppendLine($"\t\t\t\t.{nameof(WithTextFormat)}(\"{_currency.TextFormat}\")");
            }

            builder.AppendLine("\t\t\t\t.Commit();");
        }

        public ISeedGenerator<SeedBuilder.LitiumGraphQlModel.Globalization.Currency> Update(SeedBuilder.LitiumGraphQlModel.Globalization.Currency data)
        {
            if (data.IsBaseCurrency.HasValue)
            {
               _currency.IsBaseCurrency = data.IsBaseCurrency.Value;
            }

            _currency.Symbol = data.Symbol;

            if (Enum.TryParse(data.SymbolPosition.ToString(), out Currency.Positions position))
            {
                _currency.SymbolPosition = position;
            }

            _currency.ExchangeRate = data.ExchangeRate;
            _currency.GroupSeparator = data.GroupSeparator;
            _currency.TextFormat = data.TextFormat;

            return this;
        }
    }
}