using Distancify.Migrations.Litium.SeedBuilder.LitiumGraphQlModel.FieldFramework.Fields.Items;
using Litium.FieldFramework.FieldTypes;
using System.Collections.Generic;

namespace Distancify.Migrations.Litium.SeedBuilder.LitiumGraphQlModel.FieldFramework.Fields
{
    public class CompositeFieldOption
    {
        public List<TextOptionItem> Items { get; set; }

        public string EntityType { get; set; }

        public bool MultiSelect { get; set; }
    }
}
