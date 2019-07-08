using Distancify.Migrations.Litium.SeedBuilder.LitiumGraphqlModel;
using Distancify.Migrations.Litium.Seeds.Products;
using System.Text;

namespace Distancify.Migrations.Litium.SeedBuilder.Respositories
{
    public class AssortmentRepository : Repository<Assortment, AssortmentSeed>
    {
        protected override AssortmentSeed CreateFrom(Assortment graphQlItem)
        {
            return AssortmentSeed.CreateFrom(graphQlItem);
        }
    }
}
