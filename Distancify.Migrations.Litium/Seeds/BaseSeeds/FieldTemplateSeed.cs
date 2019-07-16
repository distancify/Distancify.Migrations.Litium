using Distancify.Migrations.Litium.Extensions;
using Distancify.Migrations.Litium.Seeds;
using Litium;
using Litium.Blocks;
using Litium.Customers;
using Litium.FieldFramework;
using Litium.Globalization;
using Litium.Products;
using Litium.Runtime;
using Litium.Websites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

        protected void WriteFieldGroups(ICollection<FieldTemplateFieldGroup> fieldGroups, StringBuilder builder, string methodName = nameof(WithFieldGroup))
        {
            foreach (var fieldGroup in fieldGroups)
            {
                builder.AppendLine($"\t\t\t\t.{methodName}(\"{fieldGroup.Id}\", " +
                                   $"\r\n\t\t\t\t\tnew List<string>{{{string.Join(", ", fieldGroup.Fields.Select(f => $"\"{f}\""))}}}," +
                                   $"\r\n\t\t\t\t\t{fieldGroup.Localizations.ToDictionary(k => k.Key, v => v.Value.Name).GetMigration(5)}" +
                                   $", {fieldGroup.Collapsed.ToString().ToLower()})");
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
                case BlockFieldTemplate blockFieldTemplate:
                    if (blockFieldTemplate.FieldGroups == null)
                    {
                        blockFieldTemplate.FieldGroups = new List<FieldTemplateFieldGroup>();
                    }
                    return blockFieldTemplate.FieldGroups;

                case CategoryFieldTemplate categoryFieldTemplate:
                    if (categoryFieldTemplate.CategoryFieldGroups == null)
                    {
                        categoryFieldTemplate.CategoryFieldGroups = new List<FieldTemplateFieldGroup>();
                    }
                    return categoryFieldTemplate.CategoryFieldGroups;

                case ChannelFieldTemplate channelFieldTemplate:
                    if (channelFieldTemplate.FieldGroups == null)
                    {
                        channelFieldTemplate.FieldGroups = new List<FieldTemplateFieldGroup>();
                    }
                    return channelFieldTemplate.FieldGroups;

                case MarketFieldTemplate marketFieldTemplate:
                    if (marketFieldTemplate.FieldGroups == null)
                    {
                        marketFieldTemplate.FieldGroups = new List<FieldTemplateFieldGroup>();
                    }

                    return marketFieldTemplate.FieldGroups;

                case OrganizationFieldTemplate organizationFieldTemplate:
                    if (organizationFieldTemplate.FieldGroups == null)
                    {
                        organizationFieldTemplate.FieldGroups = new List<FieldTemplateFieldGroup>();
                    }

                    return organizationFieldTemplate.FieldGroups;

                case PageFieldTemplate pageFieldTemplate:
                    if (pageFieldTemplate.FieldGroups == null)
                    {
                        pageFieldTemplate.FieldGroups = new List<FieldTemplateFieldGroup>();
                    }

                    return pageFieldTemplate.FieldGroups;

                case GroupFieldTemplate groupFieldTemplate:
                    if (groupFieldTemplate.FieldGroups == null)
                    {
                        groupFieldTemplate.FieldGroups = new List<FieldTemplateFieldGroup>();
                    }

                    return groupFieldTemplate.FieldGroups;

                case PersonFieldTemplate personFieldTemplate:
                    if (personFieldTemplate.FieldGroups == null)
                    {
                        personFieldTemplate.FieldGroups = new List<FieldTemplateFieldGroup>();
                    }
                    return personFieldTemplate.FieldGroups;


                case ProductFieldTemplate productFieldTemplate:
                    if (productFieldTemplate.ProductFieldGroups == null)
                    {
                        productFieldTemplate.ProductFieldGroups = new List<FieldTemplateFieldGroup>();
                    }

                    return productFieldTemplate.ProductFieldGroups;

                case WebsiteFieldTemplate websiteFieldTemplate:
                    if (websiteFieldTemplate.FieldGroups == null)
                    {
                        websiteFieldTemplate.FieldGroups = new List<FieldTemplateFieldGroup>();
                    }

                    return websiteFieldTemplate.FieldGroups;

                default:
                    throw new NotSupportedException("Unknown field template type when building field groups");
            }
        }
    }
}