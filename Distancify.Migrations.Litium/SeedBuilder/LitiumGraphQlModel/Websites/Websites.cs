using System.Collections.Generic;

namespace Distancify.Migrations.Litium.SeedBuilder.LitiumGraphQlModel.Websites
{
    public class RootWebsite
    {
        public IEnumerable<Website> Websites { get; set; }
        public IEnumerable<FieldDefinition> FieldDefinitions { get; set; }

        public IEnumerable<PageFieldTemplate> PageFieldTemplates { get; set; }
        public IEnumerable<WebsiteFieldTemplate> WebsiteFieldTemplates { get; set; }

    }
}
