using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distancify.Migrations.Litium.SeedBuilder.LitiumGraphQlModel
{
    public class DecimalOption
    {
        public List<DecimalOptionItem> Items { get; set; }

        public bool ManualStort { get; set; }
        public bool MultiSelect { get; set; }
    }
}
