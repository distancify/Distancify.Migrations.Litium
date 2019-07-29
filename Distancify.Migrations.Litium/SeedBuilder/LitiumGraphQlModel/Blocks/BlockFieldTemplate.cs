using Distancify.Migrations.Litium.SeedBuilder.LitiumGraphQlModel.FieldFramework;
using System;
using System.Collections.Generic;

namespace Distancify.Migrations.Litium.SeedBuilder.LitiumGraphQlModel.Blocks
{
    public class BlockFieldTemplate : FieldTemplate
    {
        public string TemplatePath { get; set; }
        public Guid CategorySystemId { get; set; }

        public List<FieldTemplateFieldGroup> FieldGroups { get; set; }
    }
}
