using System;

namespace Distancify.Migrations.Litium.SeedBuilder.LitiumGraphQlModel.Globalization
{
    public class Country : GraphQlObject
    {
        public Guid SystemId { get; set; }
        public Currency Currency { get; set; }
        public decimal? StandardVatRate { get; set; }
    }
}
