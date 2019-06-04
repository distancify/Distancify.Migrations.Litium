using Litium;
using Litium.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distancify.Migrations.Litium.Settings.Products
{
    public class AssortmentSeed : ISeed
    {
        private readonly Assortment assortment;

        protected AssortmentSeed(Assortment assortment)
        {
            this.assortment = assortment;
        }

        public void Commit()
        {
            var service = IoC.Resolve<AssortmentService>();

            if (assortment.SystemId == null || assortment.SystemId == Guid.Empty)
            {
                assortment.SystemId = Guid.NewGuid();
                service.Create(assortment);
                return;
            }
            
            service.Update(assortment);
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
            if (!assortment.Localizations.Any(l => l.Key.Equals(culture)) ||
                !assortment.Localizations[culture].Name.Equals(name))
            {
                assortment.Localizations[culture].Name = name;
            }

            return this;
        }
    }
}
