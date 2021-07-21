using System;
using System.Linq;
using System.Text;
using Litium;
using Litium.Products;

namespace Distancify.Migrations.Litium.Seeds.Products
{
    public class AssortmentSeed : ISeed
    {
        private readonly Assortment _assortment;
        private readonly bool isNew;

        protected AssortmentSeed(Assortment assortment, bool isNew)
        {
            _assortment = assortment;
            this.isNew = isNew;
        }

        public Guid Commit()
        {
            var service = IoC.Resolve<AssortmentService>();

            if (this.isNew)
            {
                if (_assortment.SystemId == Guid.Empty)
                {
                    _assortment.SystemId = Guid.NewGuid();
                }
                service.Create(_assortment);
            }
            else
            {
                service.Update(_assortment);
            }

            return _assortment.SystemId;
        }

        public static AssortmentSeed Ensure(string assortmentId)
        {
            var assortment = IoC.Resolve<AssortmentService>().Get(assortmentId)?.MakeWritableClone();
            var isNew = false;

            if (assortment is null)
            {
                assortment = new Assortment()
                {
                    Id = assortmentId,
                    SystemId = Guid.NewGuid()
                };
                isNew = true;
            }
            else
            {
                assortment = assortment.MakeWritableClone();
            }

            return new AssortmentSeed(assortment, isNew);
        }

        public static AssortmentSeed Ensure(Guid assortmentSystemId)
        {
            var assortment = IoC.Resolve<AssortmentService>().Get(assortmentSystemId)?.MakeWritableClone();
            var isNew = false;

            if (assortment is null)
            {
                assortment = new Assortment()
                {
                    SystemId = assortmentSystemId
                };
                isNew = true;
            }
            else
            {
                assortment = assortment.MakeWritableClone();
            }

            return new AssortmentSeed(assortment, isNew);
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
    }
}
