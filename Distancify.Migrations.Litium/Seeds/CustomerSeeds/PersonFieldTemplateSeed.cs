using Distancify.Migrations.Litium.Seeds.BaseSeeds;
using Litium;
using Litium.FieldFramework;
using Litium.Customers;
using System;
using System.Collections.Generic;

namespace Distancify.Migrations.Litium.Seeds.CustomerSeeds
{
    public class PersonFieldTemplateSeed : FieldTemplateSeed<PersonFieldTemplate>
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

    }
}
