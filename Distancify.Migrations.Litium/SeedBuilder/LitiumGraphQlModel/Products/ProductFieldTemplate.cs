using Distancify.Migrations.Litium.SeedBuilder.LitiumGraphQlModel.FieldFramework;
using System;
using System.Collections.Generic;

namespace Distancify.Migrations.Litium.SeedBuilder.LitiumGraphQlModel.Products
{
    public class ProductFieldTemplate : FieldTemplate
    {
        public ProductDisplayTemplate DisplayTemplate { get; set; }
        public Guid DisplayTemplateSystemId { get; set; }
        public List<FieldTemplateFieldGroup> ProductFieldGroups { get; set; }
        public List<FieldTemplateFieldGroup> VariantFieldGroups { get; set; }
    }
}
