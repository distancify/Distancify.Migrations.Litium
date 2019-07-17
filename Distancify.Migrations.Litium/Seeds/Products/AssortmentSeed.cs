using Litium;
using Litium.Products;
using System;
using System.Linq;
using System.Text;

namespace Distancify.Migrations.Litium.Seeds.Products
{
    public class AssortmentSeed : ISeed, ISeedGenerator<SeedBuilder.LitiumGraphQlModel.Assortment>
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

        internal static AssortmentSeed CreateFrom(SeedBuilder.LitiumGraphQlModel.Assortment assortment)
        {
            var seed = new AssortmentSeed(new Assortment());
            return (AssortmentSeed)seed.Update(assortment);
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

        public ISeedGenerator<SeedBuilder.LitiumGraphQlModel.Assortment> Update(SeedBuilder.LitiumGraphQlModel.Assortment data)
        {
            this.assortment.Id = data.Id;
            return this;
        }

        public void WriteMigration(StringBuilder builder)
        {
            builder.AppendLine($"\t\t\t{nameof(AssortmentSeed)}.{nameof(AssortmentSeed.Ensure)}(\"{assortment.Id}\")");
            builder.AppendLine("\t\t\t\t.Commit();");
        }
    }
}
