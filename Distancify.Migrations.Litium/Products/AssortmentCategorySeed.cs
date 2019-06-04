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
    public class AssortmentCategorySeed : ISeed
    {
        private readonly Category category;

        protected AssortmentCategorySeed(Category category)
        {
            this.category = category;
        }

        public static AssortmentCategorySeed Ensure(string assortmentCategoryId, string productFieldTemplateId, string assortmentId)
        {
            var fieldTemplateSystemGuid = IoC.Resolve<FieldTemplateService>().Get<ProductFieldTemplate>(productFieldTemplateId).SystemId;
            var assortmentSystemGuid = IoC.Resolve<AssortmentService>().Get(assortmentId).SystemId;
            var categoryClone = IoC.Resolve<CategoryService>().Get(assortmentCategoryId)?.MakeWritableClone() ??
                new Category(fieldTemplateSystemGuid, assortmentSystemGuid)
                {
                    SystemId = Guid.Empty,
                    Id = assortmentCategoryId
                };

            return new AssortmentCategorySeed(categoryClone);
        }

        public void Commit()
        {
            var service = IoC.Resolve<CategoryService>();

            if (category.SystemId == null || category.SystemId == Guid.Empty)
            {
                category.SystemId = Guid.NewGuid();
                service.Create(category);
                return;
            }

            service.Update(category);
        }
    }
}
