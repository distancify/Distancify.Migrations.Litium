using Distancify.Migrations.Litium.SeedBuilder.LitiumGraphQlModel.Products;
using Distancify.Migrations.Litium.Seeds.Products;

namespace Distancify.Migrations.Litium.SeedBuilder.Repositories
{
    public class UnitOfMeasurementRepository : Repository<UnitOfMeasurement, UnitOfMeasurementSeed>
    {
        protected override UnitOfMeasurementSeed CreateFrom(UnitOfMeasurement graphQlItem)
        {
            return UnitOfMeasurementSeed.CreateFrom(graphQlItem);
        }
    }
}
