using Distancify.Migrations.Litium.Seeds.BaseSeeds;
using Litium;
using Litium.Products;
using System;
using System.Text;

namespace Distancify.Migrations.Litium.Seeds.Product
{
    public class CategoryDisplayTemplateSeed : DisplayTemplateSeed<CategoryDisplayTemplate>, ISeedGenerator<SeedBuilder.LitiumGraphQlModel.Products.CategoryDisplayTemplate>
    {
        public CategoryDisplayTemplateSeed(CategoryDisplayTemplate categoryDisplayTemplate):base(categoryDisplayTemplate)
        {
        }

        public static CategoryDisplayTemplateSeed Ensure(string id)
        {
            var displayTemplateClone = IoC.Resolve<DisplayTemplateService>().Get<CategoryDisplayTemplate>(id)?.MakeWritableClone();
            if (displayTemplateClone is null)
            {
                displayTemplateClone = new CategoryDisplayTemplate
                {
                    Id = id,
                    SystemId = Guid.Empty
                };
            }

            return new CategoryDisplayTemplateSeed(displayTemplateClone);
        }

        public ISeedGenerator<SeedBuilder.LitiumGraphQlModel.Products.CategoryDisplayTemplate> Update(SeedBuilder.LitiumGraphQlModel.Products.CategoryDisplayTemplate data)
        {

            return this;
        }

        public void WriteMigration(StringBuilder builder)
        {
            throw new NotImplementedException();
        }
    }
}
