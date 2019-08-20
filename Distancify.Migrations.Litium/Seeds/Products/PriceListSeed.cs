using System;
using System.Linq;
using Litium;
using Litium.Globalization;
using Litium.Products;

namespace Distancify.Migrations.Litium.Seeds.Products
{
    public class PriceListSeed : ISeed
    {
        private readonly PriceList priceList;

        protected PriceListSeed(PriceList variant)
        {
            priceList = variant;
        }

        public Guid Commit()
        {
            var service = IoC.Resolve<PriceListService>();

            if (priceList.SystemId == null || priceList.SystemId == Guid.Empty)
            {
                priceList.SystemId = Guid.NewGuid();
                service.Create(priceList);
            }
            else
            {
                service.Update(priceList);
            }

            return priceList.SystemId;
        }

        public static PriceListSeed Ensure(string priceListId, string currencyId)
        {
            var currencySystemGuid = IoC.Resolve<CurrencyService>().Get(currencyId).SystemId;
            var priceListClone = IoC.Resolve<PriceListService>().Get(priceListId)?.MakeWritableClone() ??
                new PriceList(currencySystemGuid)
                {
                    SystemId = Guid.Empty,
                    Id = priceListId
                };
            return new PriceListSeed(priceListClone);
        }

        public PriceListSeed WithName(string culture, string name)
        {
            if (!priceList.Localizations.Any(l => l.Key.Equals(culture)) ||
                string.IsNullOrEmpty(priceList.Localizations[culture].Name) ||
                !priceList.Localizations[culture].Name.Equals(name))
            {
                priceList.Localizations[culture].Name = name;
            }

            return this;
        }

        public PriceListSeed WithStartDateTimeUtc(DateTimeOffset? startDateTimeUtc)
        {
            priceList.StartDateTimeUtc = startDateTimeUtc;
            return this;
        }

        public PriceListSeed WithEndDateTimeUtc(DateTimeOffset? endDateTimeUtc)
        {
            priceList.EndDateTimeUtc = endDateTimeUtc;
            return this;
        }

        public PriceListSeed IsActive(bool isActive)
        {
            priceList.Active = isActive;
            return this;
        }


        //TODO: CountryLinks
        //TODO: OrganizationLinks
        //TODO: WebSiteLinks
        //TODO: Priority
        //TODO: Items
        //TODO: GroupLinks
        //TODO: CustomData
        //TODO: IncludeVat
        //TODO: Fields
    }
}
