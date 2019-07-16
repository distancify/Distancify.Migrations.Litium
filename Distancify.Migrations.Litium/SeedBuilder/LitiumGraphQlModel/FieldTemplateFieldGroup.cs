using System.Collections.Generic;

namespace Distancify.Migrations.Litium.SeedBuilder.LitiumGraphQlModel
{
    public class FieldTemplateFieldGroup : GraphQlObject
    {
        public bool Collapsed { get; set; }
        public List<FieldLocalization> Localizations { get; set; }
        public List<string> Fields { get; set; }
    }
}
