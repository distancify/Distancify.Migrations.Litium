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
    public class CategoryFieldTemplateSeed : FieldTemplateSeed<CategoryFieldTemplate>, ISeedGenerator<SeedBuilder.LitiumGraphQlModel.Products.CategoryFieldTemplate>
    {
        public CategoryFieldTemplateSeed(CategoryFieldTemplate fieldTemplate) : base(fieldTemplate)
        {
        }

        public static CategoryFieldTemplateSeed Ensure(string id, string categoryDisplayTemplateId)
        {
            var categoryDisplayTemplateSystemGuid = IoC.Resolve<DisplayTemplateService>().Get<CategoryDisplayTemplate>(categoryDisplayTemplateId).SystemId;
            var categoryFieldTemplate = IoC.Resolve<FieldTemplateService>().Get<CategoryFieldTemplate>(id)?.MakeWritableClone();
            if (categoryFieldTemplate is null)
            {
                categoryFieldTemplate = new CategoryFieldTemplate(id, categoryDisplayTemplateSystemGuid)
                {
                    SystemId = Guid.Empty,
                    CategoryFieldGroups = new List<FieldTemplateFieldGroup>()
                };
            }

            return new CategoryFieldTemplateSeed(categoryFieldTemplate);
        }

        public static CategoryFieldTemplateSeed CreateFrom(SeedBuilder.LitiumGraphQlModel.Products.CategoryFieldTemplate categoryFieldTemplate)
        {
            var seed = new CategoryFieldTemplateSeed(new CategoryFieldTemplate(categoryFieldTemplate.Id, categoryFieldTemplate.DisplayTemplateSystemId));
            return (CategoryFieldTemplateSeed)seed.Update(categoryFieldTemplate);
        }

        public ISeedGenerator<SeedBuilder.LitiumGraphQlModel.Products.CategoryFieldTemplate> Update(SeedBuilder.LitiumGraphQlModel.Products.CategoryFieldTemplate data)
        {
            fieldTemplate.SystemId = data.SystemId;
            fieldTemplate.CategoryFieldGroups = new List<FieldTemplateFieldGroup>();

            foreach (var fieldGroup in data.FieldGroups)
            {
                AddOrUpdateFieldGroup(fieldTemplate.CategoryFieldGroups, fieldGroup.Id, fieldGroup.Fields,
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

            builder.AppendLine($"\r\n\t\t\t{nameof(CategoryDisplayTemplateSeed)}.{nameof(CategoryDisplayTemplateSeed.Ensure)}(\"{fieldTemplate.Id}\", " +
                               $"Guid.Parse(\"{fieldTemplate.DisplayTemplateSystemId.ToString()}\"))");

            foreach (var localization in fieldTemplate.Localizations)
            {
                builder.AppendLine($"\t\t\t\t.{nameof(WithName)}(\"{localization.Key}\", \"{localization.Value.Name}\")");
            }

            WriteFieldGroups(fieldTemplate.CategoryFieldGroups, builder);

            builder.AppendLine("\t\t\t\t.Commit();");
        }
    }
}
