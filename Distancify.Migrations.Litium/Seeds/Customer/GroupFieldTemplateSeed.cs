using Distancify.Migrations.Litium.Seeds.BaseSeeds;
using Litium;
using Litium.FieldFramework;
using Litium.Customers;
using System;
using System.Collections.Generic;

namespace Distancify.Migrations.Litium.Seeds.Customer
{
    public class GroupFieldTemplateSeed : FieldTemplateSeed<GroupFieldTemplate>
    {

        protected GroupFieldTemplateSeed(GroupFieldTemplate fieldTemplate)
            : base(fieldTemplate)
        {
        }

        public static GroupFieldTemplateSeed Ensure(string id)
        {
            var fieldTemplate = IoC.Resolve<FieldTemplateService>().Get<GroupFieldTemplate>(id)?.MakeWritableClone();

            if (fieldTemplate is null)
            {
                fieldTemplate = new GroupFieldTemplate(id);
                fieldTemplate.SystemId = Guid.Empty;
                fieldTemplate.FieldGroups = new List<FieldTemplateFieldGroup>();
            }

            return new GroupFieldTemplateSeed(fieldTemplate);
        }

    }
}