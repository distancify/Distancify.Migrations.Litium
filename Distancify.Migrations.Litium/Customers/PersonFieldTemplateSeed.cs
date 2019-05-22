
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
    public class PersonFieldTemplateSeed : FieldTemplateSeed
    {
        private readonly PersonFieldTemplate fieldTemplate;

        protected PersonFieldTemplateSeed(PersonFieldTemplate fieldTemplate)
            : base(fieldTemplate)
            {
            this.fieldTemplate = fieldTemplate;
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
