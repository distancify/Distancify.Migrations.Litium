using System;
using System.Linq;
using Litium;
using Litium.Products;

namespace Distancify.Migrations.Litium.Seeds.Products
{
    public class PriceListItemSeed : ISeed
    {
        private PriceListItem priceListItem;

        public PriceListItemSeed(PriceListItem inventoryItem)
        {
            this.priceListItem = inventoryItem;
        }

        public Guid Commit()
        {
            var service = IoC.Resolve<PriceListItemService>();

            if (priceListItem.SystemId == null || priceListItem.SystemId == Guid.Empty)
            {
                priceListItem.SystemId = Guid.NewGuid();
                service.Create(priceListItem);
            }
            else
            {
                service.Update(priceListItem);
            }

            return priceListItem.SystemId;
        }

        public static PriceListItemSeed Ensure(string priceListId, string variantId)
        {
            return Ensure(priceListId, variantId, default);
        }

        public static PriceListItemSeed Ensure(string priceListId, string variantId, decimal minimumQuantity)
        {
            var priceListSystemId = IoC.Resolve<PriceListService>().Get(priceListId).SystemId;
            var variantSystemId = IoC.Resolve<VariantService>().Get(variantId).SystemId;
            
            var priceListItem = IoC.Resolve<PriceListItemService>().Get(variantSystemId, priceListSystemId)
                .FirstOrDefault(i=>i.MinimumQuantity == minimumQuantity)?.MakeWritableClone() ??
                new PriceListItem(variantSystemId, priceListSystemId)
                {
                    SystemId = Guid.Empty,
                    MinimumQuantity = minimumQuantity
                };

            return new PriceListItemSeed(priceListItem);
        }

        public PriceListItemSeed WithMinimumQuantity(decimal minimumQuantity)
        {
            priceListItem.MinimumQuantity = minimumQuantity;
            return this;
        }

        public PriceListItemSeed WithPrice(decimal price)
        {
            priceListItem.Price = price;
            return this;
        }
    }
}
