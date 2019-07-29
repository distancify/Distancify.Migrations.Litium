using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distancify.Migrations.Litium.SeedBuilder.LitiumGraphQlModel.FieldFramework.Fields
{
    public class MultiFieldOption
    {
        public List<string> Fields { get; set; }
        public bool IsArray { get; set; }
    }
}
