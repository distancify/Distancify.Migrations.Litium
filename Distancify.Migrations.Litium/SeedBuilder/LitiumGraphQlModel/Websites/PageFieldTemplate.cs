using Distancify.Migrations.Litium.SeedBuilder.LitiumGraphQlModel.FieldFramework;
using System.Collections.Generic;

namespace Distancify.Migrations.Litium.SeedBuilder.LitiumGraphQlModel.Websites
{
    public class PageFieldTemplate : FieldTemplate
    {
        public string TemplatePath { get; set; }
        public List<FieldTemplateFieldGroup> FieldGroups { get; set; }
        public List<BlockContainerDefinition> Containers { get; set; }
    }
}
