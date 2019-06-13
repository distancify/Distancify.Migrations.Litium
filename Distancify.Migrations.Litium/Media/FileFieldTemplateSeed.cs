
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
    public class FileFieldTemplateSeed : FieldTemplateSeed<FileFieldTemplate>
    {


        protected FileFieldTemplateSeed(FileFieldTemplate fieldTemplate)
            : base(fieldTemplate)
            {
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

        public FileFieldTemplateSeed WithTemplateType(FileTemplateType type)
        {
            fieldTemplate.TemplateType = type;

            return this;
        }
    }
}
