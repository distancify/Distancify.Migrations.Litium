using Litium;
using Litium.FieldFramework;
using Litium.Globalization;
using Litium.Runtime;
using Litium.Websites;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Distancify.Migrations.Litium
{
    public class FieldTemplateSeed : ISeed
    {
        private readonly FieldTemplate FieldTemplate;

        private FieldTemplateSeed(FieldTemplate fieldTemplate)
        {
            this.FieldTemplate = fieldTemplate;
        }

        public void Commit()
        {
            var fieldTemplateService = IoC.Resolve<FieldTemplateService>();

            if (FieldTemplate.SystemId == Guid.Empty)
            {
                FieldTemplate.SystemId = Guid.NewGuid();
                fieldTemplateService.Create(FieldTemplate);
            }
            else
            {
                fieldTemplateService.Update(FieldTemplate);
            }
        }

        public static FieldTemplateSeed Ensure<T, TArea>(string id)
            where T : FieldTemplate<TArea>
            where TArea : IArea
        {
            var fieldTemplate = IoC.Resolve<FieldTemplateService>().Get<T>(id)?.MakeWritableClone();

            if (fieldTemplate is null)
            {
                fieldTemplate = (T)Activator.CreateInstance(typeof(T), id);
                fieldTemplate.SystemId = Guid.Empty;

                switch (fieldTemplate)
                {
                    case ChannelFieldTemplate _: (fieldTemplate as ChannelFieldTemplate).FieldGroups = new List<FieldTemplateFieldGroup>(); break;
                    case MarketFieldTemplate _: (fieldTemplate as MarketFieldTemplate).FieldGroups = new List<FieldTemplateFieldGroup>(); break;
                    case WebsiteFieldTemplate _: (fieldTemplate as WebsiteFieldTemplate).FieldGroups = new List<FieldTemplateFieldGroup>(); break;
                    case PageFieldTemplate _: (fieldTemplate as PageFieldTemplate).FieldGroups = new List<FieldTemplateFieldGroup>(); break;
                }
            }

            return new FieldTemplateSeed(fieldTemplate);
        }

        public FieldTemplateSeed WithNames(Dictionary<string, string> localizedNamesByCulture)
        {
            foreach (var item in localizedNamesByCulture)
            {
                if (!FieldTemplate.Localizations.Any(l => l.Key.Equals(item.Key)) ||
                    !FieldTemplate.Localizations[item.Key].Name.Equals(item.Value))
                {
                    FieldTemplate.Localizations[item.Key].Name = item.Value;
                }
            }

            return this;
        }

        public FieldTemplateSeed WithFieldGroup(string id, List<string> fieldIds, Dictionary<string, string> localizedNamesByCulture, bool collapsed = false)
        {
            var fieldGroups = GetFieldGroups();
            var fieldGroup = fieldGroups.FirstOrDefault(g => g.Id.Equals(id));

            if (fieldGroup == null)
            {
                fieldGroup = new FieldTemplateFieldGroup()
                {
                    Id = id,
                    Collapsed = collapsed,
                    Fields = fieldIds
                };
                SetFieldGroupLocalizations(fieldGroup, localizedNamesByCulture);
                fieldGroups.Add(fieldGroup);
            }
            else
            {
                if (fieldGroup.Collapsed != collapsed)
                {
                    fieldGroup.Collapsed = collapsed;
                }

                foreach (var fieldId in fieldIds)
                {
                    if (!fieldGroup.Fields.Contains(fieldId))
                    {
                        fieldGroup.Fields.Add(fieldId);
                    }
                }

                SetFieldGroupLocalizations(fieldGroup, localizedNamesByCulture);
            }

            return this;
        }

        private void SetFieldGroupLocalizations(FieldTemplateFieldGroup fieldGroup, Dictionary<string, string> localizedNamesByCulture)
        {
            foreach (var newLocalization in localizedNamesByCulture)
            {
                if (!fieldGroup.Localizations.Any(currentLocalization => currentLocalization.Key.Equals(newLocalization.Key)) ||
                    !fieldGroup.Localizations[newLocalization.Key].Name.Equals(newLocalization.Value))
                {
                    fieldGroup.Localizations[newLocalization.Key].Name = newLocalization.Value;
                }
            }
        }

        private ICollection<FieldTemplateFieldGroup> GetFieldGroups()
        {
            switch (FieldTemplate)
            {
                case ChannelFieldTemplate channelFieldTemplate: return channelFieldTemplate.FieldGroups;
                case MarketFieldTemplate marketFieldTemplate: return marketFieldTemplate.FieldGroups;
                case WebsiteFieldTemplate websiteFieldTemplate: return websiteFieldTemplate.FieldGroups;
                case PageFieldTemplate pageFieldTemplate: return pageFieldTemplate.FieldGroups;
                default: return null;
            }
        }
    }
}