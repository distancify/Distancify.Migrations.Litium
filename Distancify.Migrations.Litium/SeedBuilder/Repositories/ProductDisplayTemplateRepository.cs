using Distancify.Migrations.Litium.SeedBuilder.LitiumGraphQlModel.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distancify.Migrations.Litium.Seeds.Products;

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
