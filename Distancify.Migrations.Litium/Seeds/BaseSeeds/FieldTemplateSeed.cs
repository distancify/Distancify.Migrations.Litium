using Distancify.Migrations.Litium.Seeds;
using Litium;
using Litium.FieldFramework;
using Litium.Globalization;
using Litium.Products;
using Litium.Runtime;
using Litium.Websites;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Distancify.Migrations.Litium.Seeds.BaseSeeds
{
    public abstract class FieldTemplateSeed<T> : ISeed
        where T : FieldTemplate
    {
        protected readonly T fieldTemplate;

        protected FieldTemplateSeed(T fieldTemplate)
        {
            this.fieldTemplate = fieldTemplate;
        }

        public void Commit()
        {
            var fieldTemplateService = IoC.Resolve<FieldTemplateService>();

            if (fieldTemplate.SystemId == null || fieldTemplate.SystemId == Guid.Empty)
            {
                fieldTemplate.SystemId = Guid.NewGuid();
                fieldTemplateService.Create(fieldTemplate);
                return;
            }

            fieldTemplateService.Update(fieldTemplate);
        }

        public FieldTemplateSeed<T> WithName(string culture, string name)
        {
            if (!fieldTemplate.Localizations.Any(l => l.Key.Equals(culture)) ||
                !fieldTemplate.Localizations[culture].Name.Equals(name))
            {
                fieldTemplate.Localizations[culture].Name = name;
            }

            return this;
        }

        public FieldTemplateSeed<T> WithNames(Dictionary<string, string> localizedNamesByCulture)
        {
            foreach (var item in localizedNamesByCulture)
            {
                WithName(item.Key, item.Value);
            }

            return this;
        }

        public FieldTemplateSeed<T> WithFieldGroup(string id, List<string> fieldIds, Dictionary<string, string> localizedNamesByCulture, bool collapsed = false)
        {
            AddOrUpdateFieldGroup(GetFieldGroups(), id, fieldIds, localizedNamesByCulture, collapsed);
            return this;
        }

        protected void AddOrUpdateFieldGroup(ICollection<FieldTemplateFieldGroup> fieldGroups, string id, List<string> fieldIds,
            Dictionary<string, string> localizedNamesByCulture, bool collapsed = false)
        {
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
            switch (fieldTemplate)
            {
                case ChannelFieldTemplate channelFieldTemplate: return channelFieldTemplate.FieldGroups;
                case MarketFieldTemplate marketFieldTemplate: return marketFieldTemplate.FieldGroups;
                case WebsiteFieldTemplate websiteFieldTemplate: return websiteFieldTemplate.FieldGroups;
                case PageFieldTemplate pageFieldTemplate: return pageFieldTemplate.FieldGroups;
                case ProductFieldTemplate productFieldTemplate: return productFieldTemplate.ProductFieldGroups;
                default: return null;
            }
        }
    }
}