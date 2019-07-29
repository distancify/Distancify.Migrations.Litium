using Distancify.Migrations.Litium.Seeds.FieldFramework;
using Litium;
using Litium.FieldFramework;
using Litium.Customers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Distancify.Migrations.Litium.Seeds.Customers
{
    public class PersonFieldTemplateSeed : FieldTemplateSeed<PersonFieldTemplate>, ISeedGenerator<SeedBuilder.LitiumGraphQlModel.Customers.PersonFieldTemplate>
    {
        protected PersonFieldTemplateSeed(PersonFieldTemplate fieldTemplate)
            : base(fieldTemplate)
        {
        }

        public static PersonFieldTemplateSeed Ensure(string id)
        {
            var fieldTemplate = IoC.Resolve<FieldTemplateService>().Get<PersonFieldTemplate>(id)?.MakeWritableClone();

            if (fieldTemplate is null)
            {
                fieldTemplate = new PersonFieldTemplate(id);
                fieldTemplate.SystemId = Guid.Empty;
                fieldTemplate.FieldGroups = new List<FieldTemplateFieldGroup>();
            }

            return new PersonFieldTemplateSeed(fieldTemplate);
        }

        public static PersonFieldTemplateSeed CreateFrom(SeedBuilder.LitiumGraphQlModel.Customers.PersonFieldTemplate personFieldTemplate)
        {
            var seed = new PersonFieldTemplateSeed(new PersonFieldTemplate(personFieldTemplate.Id));
            return (PersonFieldTemplateSeed)seed.Update(personFieldTemplate);
        }

        public ISeedGenerator<SeedBuilder.LitiumGraphQlModel.Customers.PersonFieldTemplate> Update(SeedBuilder.LitiumGraphQlModel.Customers.PersonFieldTemplate data)
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
                throw new NullReferenceException("At least one Person Field Template with an ID obtained from the GraphQL endpoint is needed in order to ensure the Person Field Template");
            }

            builder.AppendLine($"\r\n\t\t\t{nameof(PersonFieldTemplateSeed)}.{nameof(PersonFieldTemplateSeed.Ensure)}(\"{fieldTemplate.Id}\")");

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
