using Litium;
using Litium.Products;
using System;
using System.Linq;
using System.Text;

namespace Distancify.Migrations.Litium.Seeds.Product
{
    public class AssortmentSeed : ISeed, ISeedGenerator<SeedBuilder.LitiumGraphQlModel.Assortment>
    {
        private readonly Assortment _assortment;

        protected AssortmentSeed(Assortment assortment)
        {
            _assortment = assortment;
        }

        public void Commit()
        {
            var service = IoC.Resolve<AssortmentService>();

            if (_assortment.SystemId == Guid.Empty)
            {
                _assortment.SystemId = Guid.NewGuid();
                service.Create(_assortment);
                return;
            }

            service.Update(_assortment);
        }

        public static AssortmentSeed Ensure(string assortment)
        {
            var assortmentClone = IoC.Resolve<AssortmentService>().Get(assortment)?.MakeWritableClone() ??
                new Assortment()
                {
                    Id = assortment,
                    SystemId = Guid.Empty
                };

            return new AssortmentSeed(assortmentClone);
        }

        public AssortmentSeed WithName(string culture, string name)
        {
            if (!_assortment.Localizations.Any(l => l.Key.Equals(culture)) ||
                !_assortment.Localizations[culture].Name.Equals(name))
            {
                _assortment.Localizations[culture].Name = name;
            }

            return this;
        }

        public static AssortmentSeed CreateFrom(SeedBuilder.LitiumGraphQlModel.Assortment assortment)
        {
            var seed = new AssortmentSeed(new Assortment());
            return (AssortmentSeed)seed.Update(assortment);
        }

        public ISeedGenerator<SeedBuilder.LitiumGraphQlModel.Assortment> Update(SeedBuilder.LitiumGraphQlModel.Assortment data)
        {
            _assortment.Id = data.Id;

            foreach (var localization in data.Localizations)
            {
                if (!string.IsNullOrEmpty(localization.Culture) && !string.IsNullOrEmpty(localization.Name))
                    _assortment.Localizations[localization.Culture].Name = localization.Name;
                else
                    this.Log().Warn("The Assortment {AssortmentId} contains a localization with an empty culture and/or name!", data.Id);
            }

            return this;
        }

        public void WriteMigration(StringBuilder builder)
        {
            builder.AppendLine($"\t\t\t{nameof(AssortmentSeed)}.{nameof(AssortmentSeed.Ensure)}(\"{_assortment.Id}\")");
            foreach (var localization in _assortment.Localizations)
                builder.AppendLine($"\t\t\t\t.{nameof(WithName)}(\"{localization.Key}\", \"{localization.Value.Name}\")");
            builder.AppendLine("\t\t\t\t.Commit();");
        }
    }
}
