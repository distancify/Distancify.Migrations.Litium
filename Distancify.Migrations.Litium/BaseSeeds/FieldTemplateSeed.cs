using Litium;
using Litium.FieldFramework;
using Litium.Globalization;
using Litium.Runtime;
using Litium.Websites;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Distancify.Migrations.Litium.BaseSeeds
{
    public abstract class FieldTemplateSeed : ISeed
    {
        private readonly FieldTemplate fieldTemplate;

        protected FieldTemplateSeed(FieldTemplate fieldTemplate)
        {
            this.fieldTemplate = fieldTemplate;
        }

        public void Commit()
        {
            var fieldTemplateService = IoC.Resolve<FieldTemplateService>();

            if (fieldTemplate.SystemId == Guid.Empty)
            {
                fieldTemplate.SystemId = Guid.NewGuid();
                fieldTemplateService.Create(fieldTemplate);
            }
            else
            {
                fieldTemplateService.Update(fieldTemplate);
            }
        }

        public FieldTemplateSeed WithName(string culture, string name)
        {
            if (!fieldTemplate.Localizations.Any(l => l.Key.Equals(culture)) ||
                !fieldTemplate.Localizations[culture].Name.Equals(name))
            {
                fieldTemplate.Localizations[culture].Name = name;
            }

            return this;
        }

        public FieldTemplateSeed WithNames(Dictionary<string, string> localizedNamesByCulture)
        {
            foreach (var item in localizedNamesByCulture)
            {
                WithName(item.Key, item.Value);
            }

            return this;
        }

        public FieldTemplateSeed WithFieldGroups(List<FieldTemplateFieldGroup> newFieldGroups)
        {
            ICollection<FieldTemplateFieldGroup> currentFieldGroups = null;

            switch (fieldTemplate)
            {
                case ChannelFieldTemplate channelFieldTemplate: currentFieldGroups = channelFieldTemplate.FieldGroups; break;
                case MarketFieldTemplate marketFieldTemplate: currentFieldGroups = marketFieldTemplate.FieldGroups; break;
                case WebsiteFieldTemplate websiteFieldTemplate: currentFieldGroups = websiteFieldTemplate.FieldGroups; break;
                case PageFieldTemplate pageFieldTemplate: currentFieldGroups = pageFieldTemplate.FieldGroups; break;
            }

            foreach (var newFieldGroup in newFieldGroups)
            {
                var currentFieldGroup = currentFieldGroups.FirstOrDefault(g => g.Id.Equals(newFieldGroup.Id));

                if (currentFieldGroup == null)
                {
                    currentFieldGroups.Add(newFieldGroup);
                }
                else
                {
                    foreach (var item in newFieldGroup.Localizations)
                    {
                        if (!currentFieldGroup.Localizations.Any(l => l.Key.Equals(item.Key)) ||
                            !currentFieldGroup.Localizations[item.Key].Name.Equals(item.Value))
                        {
                            currentFieldGroup.Localizations[item.Key].Name = item.Value.Name;
                        }
                    }

                    foreach (var field in newFieldGroup.Fields)
                    {
                        if (!currentFieldGroup.Fields.Contains(field))
                        {
                            currentFieldGroup.Fields.Add(field);
                        }
                    }
                }
            }

            return this;
        }
    }
}