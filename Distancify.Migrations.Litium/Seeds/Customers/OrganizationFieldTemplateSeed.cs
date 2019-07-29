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
    public class OrganizationFieldTemplateSeed : FieldTemplateSeed<OrganizationFieldTemplate>, ISeedGenerator<SeedBuilder.LitiumGraphQlModel.Customers.OrganizationFieldTemplate>
    {

        protected OrganizationFieldTemplateSeed(OrganizationFieldTemplate fieldTemplate)
            : base(fieldTemplate)
        {
        }

        public static OrganizationFieldTemplateSeed Ensure(string id)
        {
            var fieldTemplate = IoC.Resolve<FieldTemplateService>().Get<OrganizationFieldTemplate>(id)?.MakeWritableClone();

            if (fieldTemplate is null)
            {
                fieldTemplate = new OrganizationFieldTemplate(id);
                fieldTemplate.SystemId = Guid.Empty;
                fieldTemplate.FieldGroups = new List<FieldTemplateFieldGroup>();
            }

            return new OrganizationFieldTemplateSeed(fieldTemplate);
        }

        public static OrganizationFieldTemplateSeed CreateFrom(SeedBuilder.LitiumGraphQlModel.Customers.OrganizationFieldTemplate organizationFieldTemplate)
        {
            var seed = new OrganizationFieldTemplateSeed(new OrganizationFieldTemplate(organizationFieldTemplate.Id));
            return (OrganizationFieldTemplateSeed)seed.Update(organizationFieldTemplate);
        }

        public ISeedGenerator<SeedBuilder.LitiumGraphQlModel.Customers.OrganizationFieldTemplate> Update(SeedBuilder.LitiumGraphQlModel.Customers.OrganizationFieldTemplate data)
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
                throw new NullReferenceException("At least one Organization Field Template with an ID obtained from the GraphQL endpoint is needed in order to ensure the Organization Field Template");
            }

            builder.AppendLine($"\r\n\t\t\t{nameof(OrganizationFieldTemplateSeed)}.{nameof(OrganizationFieldTemplateSeed.Ensure)}(\"{fieldTemplate.Id}\")");

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
