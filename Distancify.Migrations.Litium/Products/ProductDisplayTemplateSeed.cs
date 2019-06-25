using Litium;
using Litium.Products;
using System;

namespace Distancify.Migrations.Litium.Products
{
    public class ProductDisplayTemplateSeed : ISeed
    {
        private DisplayTemplate productDisplayTemplate;

        public ProductDisplayTemplateSeed(ProductDisplayTemplate productDisplayTemplate)
        {
            this.productDisplayTemplate = productDisplayTemplate;
        }

        public static ProductDisplayTemplateSeed Ensure(string id)
        {
            var displayTemplateClone = IoC.Resolve<DisplayTemplateService>().Get<ProductDisplayTemplate>(id)?.MakeWritableClone();
            if (displayTemplateClone is null)
            {
                displayTemplateClone = new ProductDisplayTemplate();
                displayTemplateClone.Id = id;
                displayTemplateClone.SystemId = Guid.Empty;
            }

            return new ProductDisplayTemplateSeed(displayTemplateClone);
        }

        public void Commit()
        {
            var service = IoC.Resolve<DisplayTemplateService>();

            if (productDisplayTemplate.SystemId == null || productDisplayTemplate.SystemId == Guid.Empty)
            {
                productDisplayTemplate.SystemId = Guid.NewGuid();
                service.Create(productDisplayTemplate);
                return;
            }
            service.Update(productDisplayTemplate);
        }

        // TODO: WithDisplayTemplate
        // TODO: TemplatePath
        // TODO: Name
    }
}
