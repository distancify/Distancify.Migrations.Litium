using Newtonsoft.Json.Linq;
using System.Globalization;
using System.Text;

namespace Distancify.Migrations.Litium.Generator.Model
{
    public abstract class SeedWithFields
    {
        public JObject Fields { get; set; }

        
    }
}

