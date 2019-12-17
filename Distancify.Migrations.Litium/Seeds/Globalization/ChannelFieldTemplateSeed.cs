using Distancify.Migrations.Litium.Seeds.FieldFramework;
using Litium;
using Litium.FieldFramework;
using Litium.Globalization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Distancify.Migrations.Litium.Seeds.Globalization
{
    public class ChannelFieldTemplateSeed : FieldTemplateSeed<ChannelFieldTemplate, ChannelFieldTemplateSeed>, ISeedGenerator<SeedBuilder.LitiumGraphQlModel.Globalization.ChannelFieldTemplate>
    {
        protected override ChannelFieldTemplateSeed Me => this;

        protected ChannelFieldTemplateSeed(ChannelFieldTemplate fieldTemplate) : base(fieldTemplate)
        {
        }

        public static ChannelFieldTemplateSeed Ensure(string channelFieldTemplateId)
        {
            var channelFieldTemplate = (ChannelFieldTemplate)IoC.Resolve<FieldTemplateService>().Get<ChannelFieldTemplate>(channelFieldTemplateId)?.MakeWritableClone();
            if (channelFieldTemplate is null)
            {
                channelFieldTemplate = new ChannelFieldTemplate(channelFieldTemplateId)
                {
                    SystemId = Guid.Empty,
                    FieldGroups = new List<FieldTemplateFieldGroup>()
                };
            }

            return new ChannelFieldTemplateSeed(channelFieldTemplate);
        }

        public static ChannelFieldTemplateSeed CreateFrom(SeedBuilder.LitiumGraphQlModel.Globalization.ChannelFieldTemplate channelFieldTemplate)
        {
            var seed = new ChannelFieldTemplateSeed(new ChannelFieldTemplate(channelFieldTemplate.Id));
            return (ChannelFieldTemplateSeed)seed.Update(channelFieldTemplate);
        }

        public ISeedGenerator<SeedBuilder.LitiumGraphQlModel.Globalization.ChannelFieldTemplate> Update(SeedBuilder.LitiumGraphQlModel.Globalization.ChannelFieldTemplate data)
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

            builder.AppendLine($"\r\n\t\t\t{nameof(ChannelFieldTemplateSeed)}.{nameof(ChannelFieldTemplateSeed.Ensure)}(\"{fieldTemplate.Id}\")");

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

        // TODO: group
        // TODO: AreaType
    }
}
