using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distancify.Migrations.Litium.SeedBuilder.LitiumGraphQlModel
{
    public class TextOptionItem
    {
        public List<FieldLocalization> Localizations { get; set; }
        public string Value { get; set; }
    }
}
