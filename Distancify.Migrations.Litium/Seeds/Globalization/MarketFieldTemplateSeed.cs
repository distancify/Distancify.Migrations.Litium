using Distancify.Migrations.Litium.Seeds.BaseSeeds;
using Litium;
using Litium.FieldFramework;
using Litium.Globalization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Distancify.Migrations.Litium.Seeds.Globalization
{
    public class MarketFieldTemplateSeed : FieldTemplateSeed<MarketFieldTemplate>, ISeedGenerator<SeedBuilder.LitiumGraphQlModel.Globalization.MarketFieldTemplate>
    {
        public MarketFieldTemplateSeed(MarketFieldTemplate fieldTemplate) : base(fieldTemplate)
        {
        }

        public static MarketFieldTemplateSeed Ensure(string channelFieldTemplateId)
        {
            var marketFieldTemplate = (MarketFieldTemplate)IoC.Resolve<FieldTemplateService>().Get<MarketFieldTemplate>(channelFieldTemplateId)?.MakeWritableClone();
            if (marketFieldTemplate is null)
            {
                marketFieldTemplate = new MarketFieldTemplate(channelFieldTemplateId)
                {
                    SystemId = Guid.Empty,
                    FieldGroups = new List<FieldTemplateFieldGroup>()
                };
            }

            return new MarketFieldTemplateSeed(marketFieldTemplate);
        }

        public static MarketFieldTemplateSeed CreateFrom(SeedBuilder.LitiumGraphQlModel.Globalization.MarketFieldTemplate marketFieldTemplate)
        {
            var seed = new MarketFieldTemplateSeed(new MarketFieldTemplate(marketFieldTemplate.Id));
            return (MarketFieldTemplateSeed)seed.Update(marketFieldTemplate);
        }

        public ISeedGenerator<SeedBuilder.LitiumGraphQlModel.Globalization.MarketFieldTemplate> Update(SeedBuilder.LitiumGraphQlModel.Globalization.MarketFieldTemplate data)
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
                throw new NullReferenceException("At least one Market Field Template with an ID obtained from the GraphQL endpoint is needed in order to ensure the Market Field Template");
            }

            builder.AppendLine($"\r\n\t\t\t{nameof(MarketFieldTemplateSeed)}.{nameof(MarketFieldTemplateSeed.Ensure)}(\"{fieldTemplate.Id}\")");

            foreach (var localization in fieldTemplate.Localizations)
            {
                builder.AppendLine($"\t\t\t\t.{nameof(WithName)}(\"{localization.Key}\", \"{localization.Value.Name}\")");
            }

            WriteFieldGroups(fieldTemplate.FieldGroups, builder);

            builder.AppendLine("\t\t\t\t.Commit();");
        }
    }
}
