using Distancify.Migrations.Litium.Seeds.BaseSeeds;
using Litium;
using Litium.FieldFramework;
using Litium.Websites;
using System;
using System.Linq;

namespace Distancify.Migrations.Litium.Seeds.Websites
{
    public class PageFieldTemplateSeed : FieldTemplateSeed<PageFieldTemplate>
    {
        public PageFieldTemplateSeed(PageFieldTemplate fieldTemplate) : base(fieldTemplate)
        {
        }

        public static PageFieldTemplateSeed Ensure(string pageFieldTemplateId)
        {
            var pageFieldTemplate = (PageFieldTemplate)IoC.Resolve<FieldTemplateService>().Get<PageFieldTemplate>(pageFieldTemplateId)?.MakeWritableClone();
            if (pageFieldTemplate is null)
            {
                pageFieldTemplate = new PageFieldTemplate(pageFieldTemplateId);
                pageFieldTemplate.SystemId = Guid.Empty;
            }

            return new PageFieldTemplateSeed(pageFieldTemplate);
        }


        public PageFieldTemplateSeed WithContainer(string containerId)
        {
            if (fieldTemplate.Containers.FirstOrDefault(c => c.Id == containerId) == null)
            {
                fieldTemplate.Containers.Add(new BlockContainerDefinition() { Id = containerId });
                //TODO Name
            }

            return this;
        }
    }
}
