using Distancify.Migrations.Litium.SeedBuilder.LitiumGraphQlModel.Products;
using Distancify.Migrations.Litium.Seeds.Products;

namespace Distancify.Migrations.Litium.SeedBuilder.Repositories
{
    public class CategoryFieldTemplateRepository : Repository<CategoryFieldTemplate, CategoryFieldTemplateSeed>
    {
        protected override CategoryFieldTemplateSeed CreateFrom(CategoryFieldTemplate graphQlItem)
        {
            return CategoryFieldTemplateSeed.CreateFrom(graphQlItem);
        }
    }
}
