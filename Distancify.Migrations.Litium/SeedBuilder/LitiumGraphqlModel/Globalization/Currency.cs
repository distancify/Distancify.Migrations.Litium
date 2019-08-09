using System;

namespace Distancify.Migrations.Litium.SeedBuilder.LitiumGraphQlModel.Globalization
{
    public class Currency : GraphQlObject
    {
        public Guid SystemId { get; set; }
        public bool? IsBaseCurrency { get; set; }

        public string Symbol { get; set; }
        public int SymbolPosition { get; set; }

        public decimal ExchangeRate { get; set; }

        public string GroupSeparator { get; set; }
        public string TextFormat { get; set; }
    }
}
