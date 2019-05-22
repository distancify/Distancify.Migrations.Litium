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
    public class FolderFieldTemplateSeed : FieldTemplateSeed
    {
        private readonly FolderFieldTemplate fieldTemplate;

        protected FolderFieldTemplateSeed(FolderFieldTemplate fieldTemplate)
            : base(fieldTemplate)
            {
            this.fieldTemplate = fieldTemplate;
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
    }
}
