using Distancify.Migrations.Litium.SeedBuilder.LitiumGraphQlModel;
using Distancify.Migrations.Litium.Seeds.Globalization;

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
