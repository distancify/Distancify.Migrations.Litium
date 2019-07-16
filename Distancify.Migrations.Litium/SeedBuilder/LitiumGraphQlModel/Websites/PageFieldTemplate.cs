using System.Collections.Generic;

namespace Distancify.Migrations.Litium.SeedBuilder.LitiumGraphQlModel.Websites
{
    public class PageFieldTemplate : FieldTemplate
    {
        public string TemplatePath { get; set; }
        public List<FieldTemplateFieldGroup> FieldGroups { get; set; }
    }
}
