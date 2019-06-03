using Distancify.Migrations.Litium.BaseSeeds;
using Litium;
using Litium.FieldFramework;
using Litium.Products;
using System;

namespace Distancify.Migrations.Litium.Products
{
    public class ProductFieldTemplateSeed : FieldTemplateSeed
    {
        public ProductFieldTemplateSeed(FieldTemplate fieldTemplate) : base(fieldTemplate)
        {
        }

        public static ProductFieldTemplateSeed Ensure(string id, string productDisplayTemplateId)
        {
            var productDisplayTemplateSystemGuid = IoC.Resolve<DisplayTemplateService>().Get<ProductDisplayTemplate>(productDisplayTemplateId).SystemId;
            var productFieldTemplate = (ProductFieldTemplate)IoC.Resolve<FieldTemplateService>().Get<ProductFieldTemplate>(id)?.MakeWritableClone();
            if (productFieldTemplate is null)
            {
                productFieldTemplate = new ProductFieldTemplate(id, productDisplayTemplateSystemGuid);
                productFieldTemplate.SystemId = Guid.Empty;
            }

            return new ProductFieldTemplateSeed(productFieldTemplate);
        }

    }
}
