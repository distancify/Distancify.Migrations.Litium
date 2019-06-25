using Litium;
using Litium.Globalization;
using Litium.Products;
using System;
using System.Linq;

namespace Distancify.Migrations.Litium.Products
{
    public class InventorySeed : ISeed
    {
        private readonly Inventory inventory;

        protected InventorySeed(Inventory inventory)
        {
            this.inventory = inventory;
        }

        public void Commit()
        {
            var service = IoC.Resolve<InventoryService>();

            if (inventory.SystemId == null || inventory.SystemId == Guid.Empty)
            {
                inventory.SystemId = Guid.NewGuid();
                service.Create(inventory);
                return;
            }

            service.Update(inventory);
        }



        public static InventorySeed Ensure(string inventoryId)
        {

            Inventory inventoryClone = IoC.Resolve<InventoryService>().Get(inventoryId)?.MakeWritableClone();

            if (inventoryClone == null) {
                inventoryClone = new Inventory()
                {
                    SystemId = Guid.Empty,
                    Id = inventoryId
                };
            }

            return new InventorySeed(inventoryClone);
        }

        public InventorySeed WithName(string culture, string name)
        {
            if (!inventory.Localizations.Any(l => l.Key.Equals(culture)) ||
                string.IsNullOrEmpty(inventory.Localizations[culture].Name) ||
                !inventory.Localizations[culture].Name.Equals(name))
            {
                inventory.Localizations[culture].Name = name;
            }

            return this;
        }

        public InventorySeed WithAddress(string address1)
        {
            //TODO: Handle all parameters
            inventory.Address = new InventoryAddressInfo()
            {
                Address1 = address1
            };
            return this;
        }

        public InventorySeed WithCountry(string countryId)
        {
            var countrySystemGuid = IoC.Resolve<CountryService>().Get(countryId).SystemId;
            var countryLinkItem = inventory.CountryLinks.FirstOrDefault(c => c.CountrySystemId == countrySystemGuid);

            if (countryLinkItem == null)
            {
                inventory.CountryLinks.Add(
                    new InventoryToCountryLink(countrySystemGuid));
                return this;
            }

            return this;
        }

        public InventorySeed WithoutCountry(string countryId)
        {
            var countrySystemGuid = IoC.Resolve<CountryService>().Get(countryId).SystemId;
            var countryLinkItem = inventory.CountryLinks.FirstOrDefault(c => c.CountrySystemId == countrySystemGuid);

            if (countryLinkItem == null)
            {
                return this;
            }

            inventory.CountryLinks.Remove(countryLinkItem);
            return this;
        }

        //Address
        //CustomData
        //InventoryItems
        //Fields
    }
}
