using Litium;
using Litium.Globalization;
using System;
using System.Linq;

namespace Distancify.Migrations.Litium.Globalization
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

        public void Commit()
        {
            var fieldTemplateService = IoC.Resolve<TaxClassService>();

            if ( taxClass.SystemId == null || taxClass.SystemId == Guid.Empty)
            {
                taxClass.SystemId = Guid.NewGuid();
                fieldTemplateService.Create(taxClass);
                return;
            }

            fieldTemplateService.Update(taxClass);
        }

        public string GenerateMigration()
        {
            throw new NotImplementedException();
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
