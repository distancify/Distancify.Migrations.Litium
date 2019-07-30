using Distancify.Migrations.Litium.Seeds.FieldFramework;
using Litium;
using Litium.FieldFramework;
using Litium.Media;
using System;
using System.Collections.Generic;

namespace Distancify.Migrations.Litium.Seeds.Media
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
