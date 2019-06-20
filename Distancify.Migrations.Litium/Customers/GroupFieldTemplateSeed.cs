
using Distancify.Migrations.Litium.BaseSeeds;
using Litium;
using Litium.FieldFramework;
using Litium.Customers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distancify.Migrations.Litium.Customers
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
