using System;
using System.Collections.Generic;

namespace Distancify.Migrations.Litium.SeedBuilder.LitiumGraphQlModel
{
    public class FieldTemplate : GraphQlObject
    {
        public Guid SystemId { get; set; }

        public AreaType AreaType { get; set; }
        public List<FieldLocalization> Localizations { get; set; }
    }
}
