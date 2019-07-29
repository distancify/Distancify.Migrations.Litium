using Distancify.Migrations.Litium.Seeds.FieldFramework;
using Litium;
using Litium.FieldFramework;
using Litium.Media;
using System;
using System.Collections.Generic;

namespace Distancify.Migrations.Litium.Seeds.Media
{
    public class FolderFieldTemplateSeed : FieldTemplateSeed<FolderFieldTemplate>
    {
        protected FolderFieldTemplateSeed(FolderFieldTemplate fieldTemplate)
            : base(fieldTemplate)
        {
        }

        public static FolderFieldTemplateSeed Ensure(string id)
        {
            var fieldTemplate = IoC.Resolve<FieldTemplateService>().Get<FolderFieldTemplate>(id)?.MakeWritableClone();

            if (fieldTemplate is null)
            {
                fieldTemplate = new FolderFieldTemplate(id);
                fieldTemplate.SystemId = Guid.Empty;
                fieldTemplate.FieldGroups = new List<FieldTemplateFieldGroup>();
            }

            return new FolderFieldTemplateSeed(fieldTemplate);
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
