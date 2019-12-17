using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distancify.Migrations.Litium.Seeds.FieldFramework;
using Litium;
using Litium.FieldFramework;
using Litium.Websites;

namespace Distancify.Migrations.Litium.Seeds.Websites
{
    public class WebsiteFieldTemplateSeed : FieldTemplateSeed<WebsiteFieldTemplate, WebsiteFieldTemplateSeed>, ISeedGenerator<SeedBuilder.LitiumGraphQlModel.Websites.WebsiteFieldTemplate>
    {
        protected override WebsiteFieldTemplateSeed Me => this;

        public WebsiteFieldTemplateSeed(WebsiteFieldTemplate fieldTemplate) : base(fieldTemplate)
        {
        }

        public static WebsiteFieldTemplateSeed Ensure(string pageFieldTemplateId)
        {
            var websiteFieldTemplate = (WebsiteFieldTemplate)IoC.Resolve<FieldTemplateService>().Get<WebsiteFieldTemplate>(pageFieldTemplateId)?.MakeWritableClone();
            if (websiteFieldTemplate is null)
            {
                websiteFieldTemplate = new WebsiteFieldTemplate(pageFieldTemplateId)
                {
                    SystemId = Guid.Empty,
                    FieldGroups = new List<FieldTemplateFieldGroup>()
                };
            }
            return new WebsiteFieldTemplateSeed(websiteFieldTemplate);
        }

        public static WebsiteFieldTemplateSeed CreateFrom(SeedBuilder.LitiumGraphQlModel.Websites.WebsiteFieldTemplate websiteFieldTemplate)
        {
            var seed = new WebsiteFieldTemplateSeed(new WebsiteFieldTemplate(websiteFieldTemplate.Id));
            return (WebsiteFieldTemplateSeed)seed.Update(websiteFieldTemplate);
        }

        public ISeedGenerator<SeedBuilder.LitiumGraphQlModel.Websites.WebsiteFieldTemplate> Update(SeedBuilder.LitiumGraphQlModel.Websites.WebsiteFieldTemplate data)
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
                throw new NullReferenceException("At least one Website Field Template with an ID obtained from the GraphQL endpoint is needed in order to ensure the Website Field Template");
            }

            builder.AppendLine($"\r\n\t\t\t{nameof(WebsiteFieldTemplateSeed)}.{nameof(WebsiteFieldTemplateSeed.Ensure)}(\"{fieldTemplate.Id}\")");

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
        //TODO: areatype
    }
}
