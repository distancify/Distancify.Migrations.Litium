using Litium;
using Litium.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distancify.Migrations.Litium.Globalization
{
    public class UnitOfMeasurementSeed : ISeed
    {
        private readonly UnitOfMeasurement unitOfMeasurement;

        protected UnitOfMeasurementSeed(UnitOfMeasurement unitOfMeasurement)
        {
            this.unitOfMeasurement = unitOfMeasurement;
        }

        public void Commit()
        {
            var service = IoC.Resolve<UnitOfMeasurementService>();

            if (unitOfMeasurement.SystemId == null || unitOfMeasurement.SystemId == Guid.Empty)
            {
                unitOfMeasurement.SystemId = Guid.NewGuid();
                service.Create(unitOfMeasurement);
                return;
            }

            service.Update(unitOfMeasurement);
        }

        public static UnitOfMeasurementSeed Ensure(string unitOfMeasurementId)
        {
            var unitOfMeasurementClone = IoC.Resolve<UnitOfMeasurementService>().Get(unitOfMeasurementId)?.MakeWritableClone() ??
                new UnitOfMeasurement(unitOfMeasurementId)
                {
                    SystemId = Guid.Empty
                };

            return new UnitOfMeasurementSeed(unitOfMeasurementClone);
        }

        public UnitOfMeasurementSeed WithName(string culture, string name)
        {
            if (!unitOfMeasurement.Localizations.Any(l => l.Key.Equals(culture)) ||
                string.IsNullOrEmpty(unitOfMeasurement.Localizations[culture].Name) ||
                !unitOfMeasurement.Localizations[culture].Name.Equals(name))
            {
                unitOfMeasurement.Localizations[culture].Name = name;
            }

            return this;
        }

        public string GenerateMigration()
        {
            throw new NotImplementedException();
        }

        //TODO: DecimalDigits
        //TODO: Fields
    }
}
