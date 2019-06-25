using System.Collections.Generic;

namespace Distancify.Migrations.Litium.SeedBuilder.LitiumGraphqlModel
{
    public class Data
    {
        public IEnumerable<Channel> Channels { get; set; }

        public IEnumerable<DomainName> DomainNames { get; set; }

        public IEnumerable<Currency> Currencies { get; set; }

        public IEnumerable<Country> Countries { get; set; }
        public IEnumerable<Website> Websites { get; set; }
        public IEnumerable<Assortment> Assortments { get; set; }
        public IEnumerable<Language> Languages { get; set; }


    }
}
