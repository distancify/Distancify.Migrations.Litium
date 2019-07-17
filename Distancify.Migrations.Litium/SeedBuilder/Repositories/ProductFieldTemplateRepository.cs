using Distancify.Migrations.Litium.SeedBuilder.LitiumGraphQlModel.Products;
using Distancify.Migrations.Litium.Seeds.Products;

namespace Distancify.Migrations.Litium.SeedBuilder.Repositories
{
    public class ProductFieldTemplateRepository : Repository<ProductFieldTemplate, ProductFieldTemplateSeed>
    {
        protected override ProductFieldTemplateSeed CreateFrom(ProductFieldTemplate graphQlItem)
        {
            return ProductFieldTemplateSeed.CreateFrom(graphQlItem);
        }
    }
}
