using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distancify.Migrations.Litium.Extensions;
using Distancify.Migrations.Litium.Seeds.FieldFramework;
using Litium;
using Litium.Blocks;
using Litium.FieldFramework;
using Litium.Websites;

namespace Distancify.Migrations.Litium.Seeds.Websites
{
    public class PageFieldTemplateSeed : FieldTemplateSeed<PageFieldTemplate, PageFieldTemplateSeed>, ISeedGenerator<SeedBuilder.LitiumGraphQlModel.Websites.PageFieldTemplate>
    {
        protected override PageFieldTemplateSeed Me => this;

        public PageFieldTemplateSeed(PageFieldTemplate fieldTemplate) : base(fieldTemplate)
        {
        }

        public static PageFieldTemplateSeed Ensure(string pageFieldTemplateId)
        {
            var pageFieldTemplate = (PageFieldTemplate)IoC.Resolve<FieldTemplateService>().Get<PageFieldTemplate>(pageFieldTemplateId)?.MakeWritableClone();
            if (pageFieldTemplate is null)
            {
                pageFieldTemplate = new PageFieldTemplate(pageFieldTemplateId)
                {
                    SystemId = Guid.Empty,
                    FieldGroups = new List<FieldTemplateFieldGroup>()
                };
            }

            return new PageFieldTemplateSeed(pageFieldTemplate);
        }

        public static PageFieldTemplateSeed CreateFrom(SeedBuilder.LitiumGraphQlModel.Websites.PageFieldTemplate pageFieldTemplate)
        {
            var seed = new PageFieldTemplateSeed(new PageFieldTemplate(pageFieldTemplate.Id));
            return (PageFieldTemplateSeed)seed.Update(pageFieldTemplate);
        }

        public PageFieldTemplateContainerSeed WithContainer(string containerId)
        {
            if (fieldTemplate.Containers == null)
            {
                fieldTemplate.Containers = new List<BlockContainerDefinition>();
            }

            BlockContainerDefinition container = fieldTemplate.Containers.FirstOrDefault(c => c.Id == containerId);

            if (container == null)
            {
                container = new BlockContainerDefinition()
                {
                    Id = containerId
                };
                fieldTemplate.Containers.Add(container);
            }

            return new PageFieldTemplateContainerSeed(fieldTemplate, container);
        }

        public class PageFieldTemplateContainerSeed : PageFieldTemplateSeed
        {
            private readonly BlockContainerDefinition blockContainerDefintion;

            internal PageFieldTemplateContainerSeed(PageFieldTemplate fieldTemplate, BlockContainerDefinition blockContainerDefintion) : base(fieldTemplate)
            {
                this.blockContainerDefintion = blockContainerDefintion;
            }

            public PageFieldTemplateContainerSeed WithContainerDisplayName(string culture, string displayName)
            {
                if (blockContainerDefintion.Name == null)
                {
                    blockContainerDefintion.Name = new Dictionary<string, string>();
                }

                if (blockContainerDefintion.Name.ContainsKey(culture))
                {
                    blockContainerDefintion.Name[culture] = displayName;
                }
                else
                {
                    blockContainerDefintion.Name.Add(culture, displayName);
                }
                return this;
            }
        }

        public PageFieldTemplateSeed WithTemplatePath(string templatePath)
        {
            fieldTemplate.TemplatePath = templatePath;
            return this;
        }

        public ISeedGenerator<SeedBuilder.LitiumGraphQlModel.Websites.PageFieldTemplate> Update(SeedBuilder.LitiumGraphQlModel.Websites.PageFieldTemplate data)
        {
            fieldTemplate.SystemId = data.SystemId;
            fieldTemplate.FieldGroups = new List<FieldTemplateFieldGroup>();
            fieldTemplate.TemplatePath = data.TemplatePath;

            fieldTemplate.Containers = data.Containers?.Select(c => new BlockContainerDefinition
            {
                Id = c.Id,
                Name = c.Localizations.ToDictionary(k => k.Culture, v => v.Name)
            })
            .ToList() ?? new List<BlockContainerDefinition>();

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
                    Serilog.Log.Warning("The Field Template with system id {FieldTemplateSystemId} contains a localization with an empty culture and/or name!",
                        data.SystemId.ToString());
                }
            }

            return this;
        }

        public void WriteMigration(StringBuilder builder)
        {
            if (fieldTemplate == null || string.IsNullOrEmpty(fieldTemplate.Id))
            {
                throw new NullReferenceException("At least one Page Field Template with an ID obtained from the GraphQL endpoint is needed in order to ensure the Page Field Template");
            }

            builder.AppendLine($"\r\n\t\t\t{nameof(PageFieldTemplateSeed)}.{nameof(PageFieldTemplateSeed.Ensure)}(\"{fieldTemplate.Id}\")");

            foreach (var container in fieldTemplate.Containers)
            {
                builder.AppendLine($"\t\t\t\t.{nameof(WithContainer)}(\"{container.Id}\", {container.Name.GetMigration(4)})");
            }

            if (!string.IsNullOrEmpty(fieldTemplate.TemplatePath))
            {
                builder.AppendLine($"\t\t\t\t.{nameof(WithTemplatePath)}(\"{fieldTemplate.TemplatePath}\")");
            }

            foreach (var localization in fieldTemplate.Localizations)
            {
                builder.AppendLine($"\t\t\t\t.{nameof(WithName)}(\"{localization.Key}\", \"{localization.Value.Name}\")");
            }

            WriteFieldGroups(fieldTemplate.FieldGroups, builder);

            builder.AppendLine("\t\t\t\t.Commit();");
        }

        protected override ICollection<FieldTemplateFieldGroup> GetFieldGroups()
        {
            if (fieldTemplate.FieldGroups == null)
            {
                fieldTemplate.FieldGroups = new List<FieldTemplateFieldGroup>();
            }

            return fieldTemplate.FieldGroups;
        }
    }
}
