using System;
using System.Collections.Generic;

namespace Distancify.Migrations.Litium.SeedBuilder.LitiumGraphQlModel
{
    public class Assortment : GraphQlObject
    {
        public Guid SystemId { get; set; }
        public List<FieldLocalization> Localizations { get; set; }
    }
}
