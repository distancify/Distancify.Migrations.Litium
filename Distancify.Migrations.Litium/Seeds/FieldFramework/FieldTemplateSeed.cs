using Distancify.Migrations.Litium.Extensions;
using Litium;
using Litium.Blocks;
using Litium.Customers;
using Litium.FieldFramework;
using Litium.Globalization;
using Litium.Media;
using Litium.Products;
using Litium.Websites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Distancify.Migrations.Litium.Seeds.FieldFramework
{
    public abstract class FieldTemplateSeed<T> : ISeed
        where T : FieldTemplate
    {
        protected readonly T fieldTemplate;

        protected FieldTemplateSeed(T fieldTemplate)
        {
            this.fieldTemplate = fieldTemplate;
        }

        public Guid Commit()
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

            return fieldTemplate.SystemId;
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
            if (localizedNamesByCulture == null) return;

            foreach (var newLocalization in localizedNamesByCulture)
            {
                if (!fieldGroup.Localizations.Any(currentLocalization => currentLocalization.Key.Equals(newLocalization.Key)) ||
                    !fieldGroup.Localizations[newLocalization.Key].Name.Equals(newLocalization.Value))
                {
                    fieldGroup.Localizations[newLocalization.Key].Name = newLocalization.Value;
                }
            }
        }

        protected abstract ICollection<FieldTemplateFieldGroup> GetFieldGroups();
    }
}