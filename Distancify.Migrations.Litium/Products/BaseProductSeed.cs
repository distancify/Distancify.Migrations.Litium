using Litium;
using Litium.FieldFramework;
using Litium.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distancify.Migrations.Litium.Products
{
    public class BaseProductSeed : ISeed
    {

        private readonly BaseProduct baseProduct;

        protected BaseProductSeed(BaseProduct baseProduct)
        {
            this.baseProduct = baseProduct;
        }

        public static BaseProductSeed Ensure(string productId, string productFieldTemplateId)
        {
            var fieldTemplate = IoC.Resolve<FieldTemplateService>().Get<ProductFieldTemplate>(productFieldTemplateId).SystemId;
            var productClone = IoC.Resolve<BaseProductService>().Get(productId)?.MakeWritableClone() ??
                new BaseProduct(productId, fieldTemplate)
                {
                    SystemId = Guid.Empty,
                    
                };

            return new BaseProductSeed(productClone);
        }

        public void Commit()
        {
            var service = IoC.Resolve<BaseProductService>();

            if (baseProduct.SystemId == null || baseProduct.SystemId == Guid.Empty)
            {
                baseProduct.SystemId = Guid.NewGuid();
                service.Create(baseProduct);
                return;
            }

            service.Update(baseProduct);
        }

        /*
         * Active
         * TaxClassSystemId
         * CategoryLinks
         * Fields
         * Localizations
         * ProductListLinks
         * RelationshipLinks
         */

            /*
             With
            - Image
            - Variants
            - Relations
            - Bundle
            - Plan
            - Publish
            - Price
            - Inventory
            - Workflow
            - History
            - Settings
            */
}
}
