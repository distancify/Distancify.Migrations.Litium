
using Distancify.Migrations.Litium.BaseSeeds;
using Litium;
using Litium.FieldFramework;
using Litium.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distancify.Migrations.Litium.Media
{
    public class FileFieldTemplateSeed : FieldTemplateSeed
    {
        private readonly FileFieldTemplate fieldTemplate;

        protected FileFieldTemplateSeed(FileFieldTemplate fieldTemplate)
            : base(fieldTemplate)
            {
            this.fieldTemplate = fieldTemplate;
        }

        public static FileFieldTemplateSeed Ensure(string id)
        {
            var fieldTemplate = IoC.Resolve<FieldTemplateService>().Get<FileFieldTemplate>(id)?.MakeWritableClone();

            if (fieldTemplate is null)
            {
                fieldTemplate = new FileFieldTemplate(id);
                fieldTemplate.SystemId = Guid.Empty;
                fieldTemplate.FieldGroups = new List<FieldTemplateFieldGroup>();
            }

            return new FileFieldTemplateSeed(fieldTemplate);
        }

        public void WithTemplateType(FileTemplateType type)
        {
            fieldTemplate.TemplateType = type;
        }
    }
}
