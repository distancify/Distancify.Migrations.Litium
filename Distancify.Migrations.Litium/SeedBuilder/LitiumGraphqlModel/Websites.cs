using System.Collections.Generic;
using Newtonsoft.Json;

namespace Distancify.Migrations.Litium.SeedBuilder.LitiumGraphQlModel
{
    public class RootWebsite
    {
        public IEnumerable<Website> Websites { get; set; }
    }
}
