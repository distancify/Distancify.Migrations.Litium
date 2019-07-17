using System;

namespace Distancify.Migrations.Litium.SeedBuilder.LitiumGraphQlModel
{
    public class Currency : GraphQlObject
    {
        public Guid SystemId { get; set; }

        public bool? IsBaseCurrency { get; set; }

    }
}
