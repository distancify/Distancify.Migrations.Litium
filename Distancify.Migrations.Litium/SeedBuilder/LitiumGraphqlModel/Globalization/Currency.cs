using System;

namespace Distancify.Migrations.Litium.SeedBuilder.LitiumGraphQlModel.Globalization
{
    public class Currency : GraphQlObject
    {
        public Guid SystemId { get; set; }
        public bool? IsBaseCurrency { get; set; }
    }
}
