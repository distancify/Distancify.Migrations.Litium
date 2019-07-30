using Distancify.Migrations.Litium.SeedBuilder.LitiumGraphQlModel.FieldFramework;
using System.Collections.Generic;

namespace Distancify.Migrations.Litium.SeedBuilder.LitiumGraphQlModel
{
    public class Field
    {
        public List<FieldDataLocalization> Localizations { get; set; }
        public object Value { get; set; }
        public FieldDefinition Definition { get; set; }
    }
}
