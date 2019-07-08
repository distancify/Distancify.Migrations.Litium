using Distancify.Migrations.Litium.Seeds.BaseSeeds;
using Litium;
using Litium.FieldFramework;
using Litium.Products;
using System;

namespace Distancify.Migrations.Litium.Seeds.Products
{
    public class CategoryFieldTemplateSeed : FieldTemplateSeed<CategoryFieldTemplate>
    {
        public CategoryFieldTemplateSeed(CategoryFieldTemplate fieldTemplate) : base(fieldTemplate)
        {
        }

        public static CategoryFieldTemplateSeed Ensure(string id, string categoryDisplayTemplateId)
        {
            var categoryDisplayTemplateSystemGuid = IoC.Resolve<DisplayTemplateService>().Get<CategoryDisplayTemplate>(categoryDisplayTemplateId).SystemId;
            var categoryFieldTemplate = IoC.Resolve<FieldTemplateService>().Get<CategoryFieldTemplate>(id)?.MakeWritableClone();
            if (categoryFieldTemplate is null)
            {
                categoryFieldTemplate = new CategoryFieldTemplate(id, categoryDisplayTemplateSystemGuid);
                categoryFieldTemplate.SystemId = Guid.Empty;
            }

            return new CategoryFieldTemplateSeed(categoryFieldTemplate);
        }

    }
}
