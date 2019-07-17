using System;
using Litium;
using Litium.Products;

namespace Distancify.Migrations.Litium.Seeds.Products
{
    public class InventoryItemSeed : ISeed
    {
        private InventoryItem inventoryItem;

        public InventoryItemSeed(InventoryItem inventoryItem)
        {
            this.inventoryItem = inventoryItem;
        }

        public void Commit()
        {
            var service = IoC.Resolve<InventoryItemService>();

            if (inventoryItem.SystemId == null || inventoryItem.SystemId == Guid.Empty)
            {
                inventoryItem.SystemId = Guid.NewGuid();
                service.Create(inventoryItem);

                return;
            }

            service.Update(inventoryItem);
        }

        public static InventoryItemSeed Ensure(string inventoryId, string variantId)
        {
            var inventorySystemId = IoC.Resolve<InventoryService>().Get(inventoryId).SystemId;
            var variantSystemId = IoC.Resolve<VariantService>().Get(variantId).SystemId;
            var inventoryItem = IoC.Resolve<InventoryItemService>().Get(variantSystemId, inventorySystemId)?.MakeWritableClone() ??
                new InventoryItem(variantSystemId, inventorySystemId)
                {
                    SystemId = Guid.Empty
                };

            return new InventoryItemSeed(inventoryItem);
        }

        public InventoryItemSeed WithInStockQuantity(decimal inStockQuantity)
        {
            inventoryItem.InStockQuantity = inStockQuantity;
            return this;
        }

    }
}
