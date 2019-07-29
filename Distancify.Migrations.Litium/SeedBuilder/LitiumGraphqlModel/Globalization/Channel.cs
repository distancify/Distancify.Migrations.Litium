using System;
using System.Collections.Generic;
using Distancify.Migrations.Litium.SeedBuilder.LitiumGraphQlModel.Websites;

namespace Distancify.Migrations.Litium.SeedBuilder.LitiumGraphQlModel.Globalization
{
    public class Channel : GraphQlObject
    {
        public Guid SystemId { get; set; }
        public Language ProductLanguage { get; set; }
        public Language WebsiteLanguage { get; set; }

        public Website Website { get; set; }
        public Market Market { get; set; }

        public ChannelFieldTemplate FieldTemplate { get; set; }
        public IEnumerable<ChannelDomainLink> Domains { get; set; }

        public List<FieldLocalization> Localizations { get; set; }

        public IEnumerable<Country> Countries { get; set; }

        public Dictionary<string, Field> Fields { get; set; }
    }
}
