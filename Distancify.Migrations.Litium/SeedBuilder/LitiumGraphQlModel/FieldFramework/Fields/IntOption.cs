using Distancify.Migrations.Litium.SeedBuilder.LitiumGraphQlModel.FieldFramework.Fields.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distancify.Migrations.Litium.SeedBuilder.LitiumGraphQlModel.FieldFramework.Fields
{
    public class IntOption
    {
        public List<IntOptionItem> Items { get; set; }

        public bool MultiSelect { get; set; }
        public bool ManualSort { get; set; }
    }
}
