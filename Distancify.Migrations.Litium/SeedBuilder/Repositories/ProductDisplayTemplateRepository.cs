using Distancify.Migrations.Litium.SeedBuilder.LitiumGraphQlModel.Products;
using Distancify.Migrations.Litium.Seeds.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distancify.Migrations.Litium.SeedBuilder.Repositories
{
    public class ProductDisplayTemplateRepository : Repository<ProductDisplayTemplate, ProductDisplayTemplateSeed>
    {
        protected override ProductDisplayTemplateSeed CreateFrom(ProductDisplayTemplate graphQlItem)
        {
            return ProductDisplayTemplateSeed.CreateFrom(graphQlItem);
        }
    }
}
