using System;
using System.Linq;
using System.Text;
using Litium;
using Litium.Globalization;
using Litium.Products;

namespace Distancify.Migrations.Litium.Seeds.Products
{
    public class InventorySeed : ISeed, ISeedGenerator<SeedBuilder.LitiumGraphQlModel.Products.Inventory>
    {
        private readonly Inventory _inventory;

        protected InventorySeed(Inventory inventory)
        {
            _inventory = inventory;
        }

        public Guid Commit()
        {
            var service = IoC.Resolve<InventoryService>();

            if (_inventory.SystemId == Guid.Empty)
            {
                _inventory.SystemId = Guid.NewGuid();
                service.Create(_inventory);
            }
            else
            {
                service.Update(_inventory);
            }

            return _inventory.SystemId;
        }

        public static InventorySeed Ensure(string inventoryId)
        {

            var inventoryClone = IoC.Resolve<InventoryService>().Get(inventoryId)?.MakeWritableClone();
            if (inventoryClone == null)
            {
                inventoryClone = new Inventory
                {
                    SystemId = Guid.Empty,
                    Id = inventoryId
                };
            }

            return new InventorySeed(inventoryClone);
        }

        public InventorySeed WithName(string culture, string name)
        {
            if (!_inventory.Localizations.Any(l => l.Key.Equals(culture)) ||
                string.IsNullOrEmpty(_inventory.Localizations[culture].Name) ||
                !_inventory.Localizations[culture].Name.Equals(name))
            {
                _inventory.Localizations[culture].Name = name;
            }

            return this;
        }

        public InventorySeed WithAddress(string address1 = null,
            string address2 = null,
            string city = null,
            string country = null,
            string email = null,
            string fax = null,
            string mobilePhone = null,
            string phone = null,
            string state = null,
            string zip = null)
        {   
            _inventory.Address = new InventoryAddressInfo()
            {
                Address1 = address1,
                Address2 = address2,
                City = city,
                Country = country,
                Email = email,
                Fax = fax,
                MobilePhone = mobilePhone,
                Phone = phone,
                State =state,
                Zip = zip
            };

            return this;
        }

        public InventorySeed WithCountry(string countryId)
        {
            var countrySystemGuid = IoC.Resolve<CountryService>().Get(countryId).SystemId;
            var countryLinkItem = _inventory.CountryLinks.FirstOrDefault(c => c.CountrySystemId == countrySystemGuid);

            if (countryLinkItem == null)
            {
                _inventory.CountryLinks.Add(
                    new InventoryToCountryLink(countrySystemGuid));
                return this;
            }

            return this;
        }

        public InventorySeed WithoutCountry(string countryId)
        {
            var countrySystemGuid = IoC.Resolve<CountryService>().Get(countryId).SystemId;
            var countryLinkItem = _inventory.CountryLinks.FirstOrDefault(c => c.CountrySystemId == countrySystemGuid);

            if (countryLinkItem == null)
            {
                return this;
            }

            _inventory.CountryLinks.Remove(countryLinkItem);
            return this;
        }

        //Address
        //CustomData
        //InventoryItems
        //Fields

        public static InventorySeed CreateFrom(SeedBuilder.LitiumGraphQlModel.Products.Inventory graphQlItem)
        {
            var seed = new InventorySeed(new Inventory {Id = graphQlItem.Id});
            return (InventorySeed)seed.Update(graphQlItem);
        }

        public ISeedGenerator<SeedBuilder.LitiumGraphQlModel.Products.Inventory> Update(SeedBuilder.LitiumGraphQlModel.Products.Inventory data)
        {
            if (Guid.TryParse(data.SystemId, out var systemId))
                this._inventory.SystemId = systemId;

            return this;
        }

        public void WriteMigration(StringBuilder builder)
        {
            builder.AppendLine($"\r\n\t\t\t{nameof(InventorySeed)}.{nameof(Ensure)}(\"{_inventory.Id}\")");
            builder.AppendLine("\t\t\t\t.Commit();");
        }
    }
}
