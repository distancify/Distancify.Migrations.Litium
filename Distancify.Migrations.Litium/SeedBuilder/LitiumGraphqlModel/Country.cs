using System;

namespace Distancify.Migrations.Litium.SeedBuilder.LitiumGraphQlModel
{
    public class Country : GraphQlObject
    {
        public Guid SystemId { get; set; }

        public Currency Currency { get; set; }

        //taxclass
        public decimal? StandardVatRate { get; set; }

    }
}
