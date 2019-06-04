using Litium;
using Litium.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distancify.Migrations.Litium.Products
{
    public class VariantSeed : ISeed
    {
        private readonly Variant variant;

        protected VariantSeed(Variant variant)
        {
            this.variant = variant;
        }

        public void Commit()
        {
            var service = IoC.Resolve<VariantService>();

            if (variant.SystemId == null || variant.SystemId == Guid.Empty)
            {
                variant.SystemId = Guid.NewGuid();
                service.Create(variant);
                return;
            }

            service.Update(variant);
        }

        public static VariantSeed Ensure(string variantId, string baseProductId)
        {
            var baseProductSystemGuid = IoC.Resolve<BaseProductService>().Get(baseProductId).SystemId;
            var variantClone = IoC.Resolve<VariantService>().Get(variantId)?.MakeWritableClone() ??
                new Variant(variantId, baseProductSystemGuid)
                {
                    SystemId = Guid.Empty
                };

            return new VariantSeed(variantClone);
        }

        /* TODO:
         * BundledVariants
         * BundleOfVariants
         * Fields
         * InventoryItems
         * Localizations
         * Prices
         * RelationshipLinks
         * SortIndex
         * ChannelLinks
         */
    }
}
