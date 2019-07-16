using Distancify.Migrations.Litium.Seeds.BaseSeeds;
using Litium;
using Litium.FieldFramework;
using Litium.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Distancify.Migrations.Litium.Seeds.Product
{
    public class ProductFieldTemplateSeed : FieldTemplateSeed<ProductFieldTemplate>, ISeedGenerator<SeedBuilder.LitiumGraphQlModel.Products.ProductFieldTemplate>
    {
        public ProductFieldTemplateSeed(ProductFieldTemplate fieldTemplate) : base(fieldTemplate)
        {
        }

        public static ProductFieldTemplateSeed Ensure(string id, string productDisplayTemplateId)
        {
            var productDisplayTemplateSystemGuid = IoC.Resolve<DisplayTemplateService>().Get<ProductDisplayTemplate>(productDisplayTemplateId).SystemId;
            var productFieldTemplate = IoC.Resolve<FieldTemplateService>().Get<ProductFieldTemplate>(id)?.MakeWritableClone();
            if (productFieldTemplate is null)
            {
                productFieldTemplate = new ProductFieldTemplate(id, productDisplayTemplateSystemGuid);
                productFieldTemplate.SystemId = Guid.Empty;
            }

            return new ProductFieldTemplateSeed(productFieldTemplate);
        }

        public ProductFieldTemplateSeed WithVariantFieldGroup(string id, List<string> fieldIds, Dictionary<string, string> localizedNamesByCulture, bool collapsed = false)
        {
            var fieldGroups = (fieldTemplate as ProductFieldTemplate).VariantFieldGroups;
            AddOrUpdateFieldGroup(fieldGroups, id, fieldIds, localizedNamesByCulture, collapsed);

            return this;
        }

        public static ProductFieldTemplateSeed CreateFrom(SeedBuilder.LitiumGraphQlModel.Products.ProductFieldTemplate productFieldTemplate)
        {
            var seed = new ProductFieldTemplateSeed(new ProductFieldTemplate(productFieldTemplate.Id, productFieldTemplate.DisplayTemplateSystemId));
            return (ProductFieldTemplateSeed)seed.Update(productFieldTemplate);
        }

        public ISeedGenerator<SeedBuilder.LitiumGraphQlModel.Products.ProductFieldTemplate> Update(SeedBuilder.LitiumGraphQlModel.Products.ProductFieldTemplate data)
        {
            fieldTemplate.SystemId = data.SystemId;
            fieldTemplate.ProductFieldGroups = new List<FieldTemplateFieldGroup>();
            fieldTemplate.VariantFieldGroups = new List<FieldTemplateFieldGroup>();

            foreach (var fieldGroup in data.ProductFieldGroups)
            {
                AddOrUpdateFieldGroup(fieldTemplate.ProductFieldGroups, fieldGroup.Id, fieldGroup.Fields,
                    fieldGroup.Localizations.ToDictionary(k => k.Culture, v => v.Name), fieldGroup.Collapsed);
            }

            foreach (var fieldGroup in data.VariantFieldGroups)
            {
                AddOrUpdateFieldGroup(fieldTemplate.VariantFieldGroups, fieldGroup.Id, fieldGroup.Fields,
                    fieldGroup.Localizations.ToDictionary(k => k.Culture, v => v.Name), fieldGroup.Collapsed);
            }

            foreach (var localization in data.Localizations)
            {
                if (!string.IsNullOrEmpty(localization.Culture) && !string.IsNullOrEmpty(localization.Name))
                {
                    fieldTemplate.Localizations[localization.Culture].Name = localization.Name;
                }
                else
                {
                    this.Log().Warn("The Field Template with system id {FieldTemplateSystemId} contains a localization with an empty culture and/or name!",
                        data.SystemId.ToString());
                }
            }

            return this;
        }

        public void WriteMigration(StringBuilder builder)
        {
            if (fieldTemplate == null || string.IsNullOrEmpty(fieldTemplate.Id))
            {
                throw new NullReferenceException("At least one Channel Field Template with an ID obtained from the GraphQL endpoint is needed in order to ensure the Channel Field Template");
            }

            builder.AppendLine($"\r\n\t\t\t{nameof(ProductFieldTemplateSeed)}.{nameof(ProductFieldTemplateSeed.Ensure)}(\"{fieldTemplate.Id}\", "+ 
                               $"Guid.Parse(\"{fieldTemplate.DisplayTemplateSystemId.ToString()}\"))");

            WriteFieldGroups(fieldTemplate.VariantFieldGroups, builder, nameof(WithVariantFieldGroup));
            WriteFieldGroups(fieldTemplate.ProductFieldGroups, builder);

            foreach (var localization in fieldTemplate.Localizations)
            {
                builder.AppendLine($"\t\t\t\t.{nameof(WithName)}(\"{localization.Key}\", \"{localization.Value.Name}\")");
            }

            builder.AppendLine("\t\t\t\t.Commit();");
        }
    }
}
