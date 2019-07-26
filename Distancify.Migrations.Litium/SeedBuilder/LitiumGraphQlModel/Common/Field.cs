using System.Collections.Generic;

namespace Distancify.Migrations.Litium.SeedBuilder.LitiumGraphQlModel.Common
{
    public class Field
    {
        public Dictionary<string, object> LocalizedValues { get; set; }
        public object Value { get; set; }
        public FieldDefinition Definition { get; set; }
    }
}
