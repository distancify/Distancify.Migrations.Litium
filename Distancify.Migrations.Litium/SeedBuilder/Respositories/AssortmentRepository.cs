using Distancify.Migrations.Litium.SeedBuilder.LitiumGraphQlModel;
using System.Text;
using Distancify.Migrations.Litium.Seeds.Product;

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
