using Distancify.Migrations.Litium.SeedBuilder.LitiumGraphQlModel.Products;
using Distancify.Migrations.Litium.Seeds.Products;

namespace Distancify.Migrations.Litium.SeedBuilder.Repositories
{
    public class CategoryDisplayTemplateRepository : Repository<CategoryDisplayTemplate, CategoryDisplayTemplateSeed>
    {
        protected override CategoryDisplayTemplateSeed CreateFrom(CategoryDisplayTemplate graphQlItem)
        {
            return CategoryDisplayTemplateSeed.CreateFrom(graphQlItem);
        }
    }
}
