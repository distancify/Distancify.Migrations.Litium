using Litium.FieldFramework.FieldTypes;
using System.Collections.Generic;

namespace Distancify.Migrations.Litium.SeedBuilder.LitiumGraphQlModel
{
    public class CompositeFieldOption
    {
        public List<TextOptionItem> Items { get; set; }

        public string EntityType { get; set; }

        public bool MultiSelect { get; set; }
    }
}
