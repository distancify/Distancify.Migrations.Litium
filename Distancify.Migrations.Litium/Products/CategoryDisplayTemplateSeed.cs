using Litium;
using Litium.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distancify.Migrations.Litium.Products
{
    public class CategoryDisplayTemplateSeed : ISeed
    {

        private CategoryDisplayTemplate categoryDisplayTemplate;

        public CategoryDisplayTemplateSeed(CategoryDisplayTemplate productDisplayTemplate)
        {
            this.categoryDisplayTemplate = productDisplayTemplate;
        }

        public static CategoryDisplayTemplateSeed Ensure(string id)
        {
            var displayTemplateClone = IoC.Resolve<DisplayTemplateService>().Get<CategoryDisplayTemplate>(id)?.MakeWritableClone();
            if (displayTemplateClone is null)
            {
                displayTemplateClone = new CategoryDisplayTemplate();
                displayTemplateClone.Id = id;
                displayTemplateClone.SystemId = Guid.Empty;
            }

            return new CategoryDisplayTemplateSeed(displayTemplateClone);
        }

        public void Commit()
        {
            var service = IoC.Resolve<DisplayTemplateService>();

            if (categoryDisplayTemplate.SystemId == null || categoryDisplayTemplate.SystemId == Guid.Empty)
            {
                categoryDisplayTemplate.SystemId = Guid.NewGuid();
                service.Create(categoryDisplayTemplate);
                return;
            }
            service.Update(categoryDisplayTemplate);
        }
    }
}
