using System.Collections.Generic;

namespace Distancify.Migrations.Litium.SeedBuilder.LitiumGraphQlModel.Globalization
{
    public class Globalization
    {
        public IEnumerable<FieldDefinition> FieldDefinitions { get; set; }
        public IEnumerable<Channel> Channels { get; set; }
        public IEnumerable<DomainName> DomainNames { get; set; }
        public IEnumerable<Currency> Currencies { get; set; }
        public IEnumerable<Country> Countries { get; set; }
        public IEnumerable<Language> Languages { get; set; }
    }
}
