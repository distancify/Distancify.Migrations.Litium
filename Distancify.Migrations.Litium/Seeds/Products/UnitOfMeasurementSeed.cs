using System;
using System.Linq;
using System.Text;
using Litium;
using Litium.Products;

namespace Distancify.Migrations.Litium.Seeds.Products
{
    public class UnitOfMeasurementSeed : ISeed, ISeedGenerator<SeedBuilder.LitiumGraphQlModel.Products.UnitOfMeasurement>
    {
        private readonly UnitOfMeasurement _unitOfMeasurement;

        protected UnitOfMeasurementSeed(UnitOfMeasurement unitOfMeasurement)
        {
            _unitOfMeasurement = unitOfMeasurement;
        }

        public void Commit()
        {
            var service = IoC.Resolve<UnitOfMeasurementService>();
            if (_unitOfMeasurement.SystemId == Guid.Empty)
            {
                _unitOfMeasurement.SystemId = Guid.NewGuid();
                service.Create(_unitOfMeasurement);
                return;
            }

            service.Update(_unitOfMeasurement);
        }

        public static UnitOfMeasurementSeed Ensure(string id)
        {
            var unitOfMeasurement =
                IoC.Resolve<UnitOfMeasurementService>().Get(id)?.MakeWritableClone() ??
                new UnitOfMeasurement(id)
                {
                    SystemId = Guid.Empty
                };

            return new UnitOfMeasurementSeed(unitOfMeasurement);
        }

        public UnitOfMeasurementSeed WithName(string culture, string name)
        {
            if (!_unitOfMeasurement.Localizations.Any(l => l.Key.Equals(culture)) ||
                string.IsNullOrEmpty(_unitOfMeasurement.Localizations[culture].Name) ||
                !_unitOfMeasurement.Localizations[culture].Name.Equals(name))
            {
                _unitOfMeasurement.Localizations[culture].Name = name;
            }

            return this;
        }

        public UnitOfMeasurementSeed WithDecimalDigits(int decimalDigits)
        {
            if (decimalDigits < 0 || decimalDigits > 4)
                throw new ArgumentOutOfRangeException(nameof(decimalDigits), "The value must be a number from the interval 0-4");

            _unitOfMeasurement.DecimalDigits = decimalDigits;
            return this;
        }

        //TODO: Fields

        internal static UnitOfMeasurementSeed CreateFrom(SeedBuilder.LitiumGraphQlModel.Products.UnitOfMeasurement graphQlItem)
        {
            var seed = new UnitOfMeasurementSeed(new UnitOfMeasurement(graphQlItem.Id));
            return (UnitOfMeasurementSeed)seed.Update(graphQlItem);
        }

        public ISeedGenerator<SeedBuilder.LitiumGraphQlModel.Products.UnitOfMeasurement> Update(SeedBuilder.LitiumGraphQlModel.Products.UnitOfMeasurement data)
        {
            if (Guid.TryParse(data.SystemId, out var systemId))
                _unitOfMeasurement.SystemId = systemId;

            _unitOfMeasurement.DecimalDigits = data.DecimalDigits;

            foreach (var localization in data.Localizations)
            {
                if (!string.IsNullOrEmpty(localization.Culture) && !string.IsNullOrEmpty(localization.Name))
                    _unitOfMeasurement.Localizations[localization.Culture].Name = localization.Name;
                else 
                    this.Log().Warn("The UnitOfMeasurement {UnitOfMeasurementId} contains a localization with an empty culture and/or name!", data.Id);
            }

            return this;
        }

        public void WriteMigration(StringBuilder builder)
        {
            builder.AppendLine($"\r\n\t\t\t{nameof(UnitOfMeasurementSeed)}.{nameof(Ensure)}(\"{_unitOfMeasurement.Id}\")");
            foreach (var localization in _unitOfMeasurement.Localizations)
                builder.AppendLine($"\t\t\t\t.{nameof(WithName)}(\"{localization.Key}\", \"{localization.Value.Name}\")");

            if(_unitOfMeasurement.DecimalDigits > 0)
                builder.AppendLine($"\t\t\t\t.{nameof(WithDecimalDigits)}({_unitOfMeasurement.DecimalDigits})");

            builder.AppendLine("\t\t\t\t.Commit();");
        }
    }
}
