using Distancify.Migrations.Litium.Seeds.BaseSeeds;
using Litium;
using Litium.FieldFramework;
using Litium.Customers;
using System;
using System.Collections.Generic;

namespace Distancify.Migrations.Litium.Seeds.Customers
{
    public class OrganizationFieldTemplateSeed : FieldTemplateSeed<OrganizationFieldTemplate>
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

    }
}
