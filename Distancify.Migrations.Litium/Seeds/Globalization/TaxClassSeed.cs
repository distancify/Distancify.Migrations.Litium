using Litium;
using Litium.Globalization;
using System;
using System.Linq;

namespace Distancify.Migrations.Litium.Seeds.Globalization
{
    public class TaxClassSeed : ISeed
    {
        private readonly TaxClass taxClass;

        protected TaxClassSeed(TaxClass taxClass)
        {
            this.taxClass = taxClass;
        }

        public static TaxClassSeed Ensure(string taxClass)
        {
            var taxClassClone = IoC.Resolve<TaxClassService>().Get(taxClass)?.MakeWritableClone() ?? new TaxClass()
            {
                SystemId = Guid.Empty,
                Id = taxClass
            };


            return new TaxClassSeed(taxClassClone);
        }

        public Guid Commit()
        {
            var fieldTemplateService = IoC.Resolve<TaxClassService>();

            if (taxClass.SystemId == null || taxClass.SystemId == Guid.Empty)
            {
                taxClass.SystemId = Guid.NewGuid();
                fieldTemplateService.Create(taxClass);
            }
            else
            {
                fieldTemplateService.Update(taxClass);
            }

            return taxClass.SystemId;
        }

        public TaxClassSeed WithName(string culture, string name)
        {
            if (!taxClass.Localizations.Any(l => l.Key.Equals(culture)) ||
                !taxClass.Localizations[culture].Name.Equals(name))
            {
                taxClass.Localizations[culture].Name = name;
            }

            return this;
        }

    }
}
