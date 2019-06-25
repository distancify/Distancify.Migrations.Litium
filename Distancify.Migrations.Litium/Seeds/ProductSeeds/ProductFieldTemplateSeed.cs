using Distancify.Migrations.Litium.Seeds.BaseSeeds;
using Litium;
using Litium.FieldFramework;
using Litium.Products;
using System;

namespace Distancify.Migrations.Litium.Seeds.ProductSeeds
{
    public class ProductFieldTemplateSeed : FieldTemplateSeed<ProductFieldTemplate>
    {
        public ProductFieldTemplateSeed(ProductFieldTemplate fieldTemplate) : base(fieldTemplate)
        {
        }

        public static ProductFieldTemplateSeed Ensure(string id, string productDisplayTemplateId)
        {
            var productDisplayTemplateSystemGuid = IoC.Resolve<DisplayTemplateService>().Get<ProductDisplayTemplate>(productDisplayTemplateId).SystemId;
            var productFieldTemplate = IoC.Resolve<FieldTemplateService>().Get<ProductFieldTemplate>(id)?.MakeWritableClone();
            if (productFieldTemplate is null)
            {
                productFieldTemplate = new ProductFieldTemplate(id, productDisplayTemplateSystemGuid);
                productFieldTemplate.SystemId = Guid.Empty;
            }

            return new ProductFieldTemplateSeed(productFieldTemplate);

        }
    }
}
