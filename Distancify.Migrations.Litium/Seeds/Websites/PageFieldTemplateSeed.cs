using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distancify.Migrations.Litium.Seeds.BaseSeeds;
using Litium;
using Litium.FieldFramework;
using Litium.Websites;

namespace Distancify.Migrations.Litium.Seeds.Websites
{
    public class PageFieldTemplateSeed : FieldTemplateSeed<PageFieldTemplate>, ISeedGenerator<SeedBuilder.LitiumGraphQlModel.Websites.PageFieldTemplate>
    {
        public PageFieldTemplateSeed(PageFieldTemplate fieldTemplate) : base(fieldTemplate)
        {
        }

        public static PageFieldTemplateSeed Ensure(string pageFieldTemplateId)
        {
            var pageFieldTemplate = (PageFieldTemplate)IoC.Resolve<FieldTemplateService>().Get<PageFieldTemplate>(pageFieldTemplateId)?.MakeWritableClone();
            if (pageFieldTemplate is null)
            {
                pageFieldTemplate = new PageFieldTemplate(pageFieldTemplateId);
                pageFieldTemplate.SystemId = Guid.Empty;
            }

            return new PageFieldTemplateSeed(pageFieldTemplate);
        }

        public static PageFieldTemplateSeed CreateFrom(SeedBuilder.LitiumGraphQlModel.Websites.PageFieldTemplate pageFieldTemplate)
        {
            var seed = new PageFieldTemplateSeed(new PageFieldTemplate(pageFieldTemplate.Id));
            return (PageFieldTemplateSeed)seed.Update(pageFieldTemplate);
        }

        public PageFieldTemplateSeed WithContainer(string containerId)
        {
            if (fieldTemplate.Containers.FirstOrDefault(c => c.Id == containerId) == null)
            {
                fieldTemplate.Containers.Add(new BlockContainerDefinition() { Id = containerId });
                //TODO Name
            }

            return this;
        }

        public ISeedGenerator<SeedBuilder.LitiumGraphQlModel.Websites.PageFieldTemplate> Update(SeedBuilder.LitiumGraphQlModel.Websites.PageFieldTemplate data)
        {
            fieldTemplate.SystemId = data.SystemId;
            fieldTemplate.FieldGroups = new List<FieldTemplateFieldGroup>();

            foreach (var fieldGroup in data.FieldGroups)
            {
                AddOrUpdateFieldGroup(fieldTemplate.FieldGroups, fieldGroup.Id, fieldGroup.Fields,
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

            builder.AppendLine($"\r\n\t\t\t{nameof(PageFieldTemplateSeed)}.{nameof(PageFieldTemplateSeed.Ensure)}(\"{fieldTemplate.Id}\")");

            foreach (var localization in fieldTemplate.Localizations)
            {
                builder.AppendLine($"\t\t\t\t.{nameof(WithName)}(\"{localization.Key}\", \"{localization.Value.Name}\")");
            }

            WriteFieldGroups(fieldTemplate.FieldGroups, builder);

            builder.AppendLine("\t\t\t\t.Commit();");
        }
    }
}
