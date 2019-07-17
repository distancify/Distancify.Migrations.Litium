using System.Collections.Generic;

namespace Distancify.Migrations.Litium.SeedBuilder.LitiumGraphQlModel
{
    public class TextOptionItem
    {
        public List<FieldLocalization> Localizations { get; set; }
        public string Value { get; set; }
    }
}
