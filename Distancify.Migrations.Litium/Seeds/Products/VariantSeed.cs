using System;
using System.Collections.Generic;
using System.Linq;
using Litium;
using Litium.Globalization;
using Litium.Products;

namespace Distancify.Migrations.Litium.Seeds.Products
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
        public VariantSeed WithName(string culture, string name)
        {
            if (!variant.Localizations.Any(l => l.Key.Equals(culture)) ||
                string.IsNullOrEmpty(variant.Localizations[culture].Name) ||
                !variant.Localizations[culture].Name.Equals(name))
            {
                variant.Localizations[culture].Name = name;
            }

            return this;
        }

        public VariantSeed WithField(string fieldName, Dictionary<string, object> values)
        {
            foreach (var localization in values.Keys)
            {
                variant.Fields.AddOrUpdateValue(fieldName, localization, values[localization]);
            }

            return this;
        }

        public VariantSeed WithField(string fieldName, object value)
        {
            variant.Fields.AddOrUpdateValue(fieldName, value);

            return this;
        }

        public VariantSeed WithDescription(string culture, string description)
        {
            if (!variant.Localizations.Any(l => l.Key.Equals(culture)) ||
                string.IsNullOrEmpty(variant.Localizations[culture].Description) ||
                !variant.Localizations[culture].Description.Equals(description))
            {
                variant.Localizations[culture].Description = description;
            }

            return this;
        }

        public VariantSeed WithSeoDescription(string culture, string seoDescription)
        {
            if (!variant.Localizations.Any(l => l.Key.Equals(culture)) ||
                string.IsNullOrEmpty(variant.Localizations[culture].SeoDescription) ||
                !variant.Localizations[culture].SeoDescription.Equals(seoDescription))
            {
                variant.Localizations[culture].SeoDescription = seoDescription;
            }

            return this;
        }

        public VariantSeed WithSeoTitle(string culture, string seoTitle)
        {
            if (!variant.Localizations.Any(l => l.Key.Equals(culture)) ||
                string.IsNullOrEmpty(variant.Localizations[culture].SeoTitle) ||
                !variant.Localizations[culture].SeoTitle.Equals(seoTitle))
            {
                variant.Localizations[culture].SeoTitle = seoTitle;
            }

            return this;
        }

        public VariantSeed WithUrl(string culture, string url)
        {
            if (!variant.Localizations.Any(l => l.Key.Equals(culture)) ||
                string.IsNullOrEmpty(variant.Localizations[culture].Url) ||
                !variant.Localizations[culture].Url.Equals(url))
            {
                variant.Localizations[culture].Url = url;
            }

            return this;
        }

        public VariantSeed WithPrice(string priceListId, decimal price)
        {
            var priceListSystemGuid = IoC.Resolve<PriceListService>().Get(priceListId).SystemId;
            var priceItem = variant.Prices.FirstOrDefault(p => p.PriceListSystemId == priceListSystemGuid);

            if (priceItem == null)
            {
                variant.Prices.Add(
                    new VariantPriceItem(priceListSystemGuid)
                    {
                        //MinimumQuantity
                        Price = price
                        //VatPercentage
                    });
                return this;
            }

            priceItem.Price = price;

            return this;
        }

        public VariantSeed WithChannelLink(string channelId)
        {
            var channelSystemId = IoC.Resolve<ChannelService>().Get(channelId).SystemId;

            if (!variant.ChannelLinks.Any(l => l.ChannelSystemId == channelSystemId))
            {
                variant.ChannelLinks.Add(new VariantToChannelLink(channelSystemId));
            }

            return this;
        }

        public VariantSeed WithUnitOfMeasurement(string unitOfMeasurementId)
        {
            var unitOfMeasurementSystemId = IoC.Resolve<UnitOfMeasurementService>().Get(unitOfMeasurementId).SystemId;

            variant.UnitOfMeasurementSystemId = unitOfMeasurementSystemId;
            return this;
        }

        //public VariantSeed WithInventoryItem(string inventoryId, decimal inStockQuantity)
        //{
        //    var inventoryItemService = IoC.Resolve<InventoryItemService>();

        //    var inventorySystemId = IoC.Resolve<InventoryService>().Get(inventoryId).SystemId;
        //    var inventoryItem = inventoryItemService.Get(variant.SystemId, inventorySystemId)?.MakeWritableClone();

        //    if (inventoryItem == null)
        //    {
        //        inventoryItemService.Create(new InventoryItem(variant.SystemId, inventorySystemId)
        //        {
        //            InStockQuantity = inStockQuantity
        //        });
        //    }
        //    else
        //    {
        //        inventoryItem.InStockQuantity = inStockQuantity;
        //        inventoryItemService.Update(inventoryItem);
        //    }

        //    return this;
        //}

        //public VariantSeed WithInventoryItem(string inventoryId, string unitOfMeasurementId, decimal inStockQuantity = 0)
        //{
        //    var inventorySystemGuid = IoC.Resolve<InventoryService>().Get(inventoryId).SystemId;
        //    return WithInventoryItem(inventorySystemGuid, unitOfMeasurementId, inStockQuantity);
        //}

        //public VariantSeed WithInventoryItem(Guid inventorySystemGuid, string unitOfMeasurementId, decimal inStockQuantity = 0)
        //{
        //    var unitOfMeasurementSystemGuid = IoC.Resolve<UnitOfMeasurementService>().Get(unitOfMeasurementId).SystemId;

        //    var inventoryItem = variant.InventoryItems.First(i => i.InventorySystemId == inventorySystemGuid && i.UnitOfMeasurementSystemId == unitOfMeasurementSystemGuid);


        //    if (inventoryItem == null)
        //    {

        //        variant.InventoryItems.Add(new VariantInventoryItem(inventorySystemGuid, unitOfMeasurementSystemGuid)
        //        {
        //            //CustomData
        //            //ErpStatus
        //            InStockQuantity = inStockQuantity
        //            //InventoryDateTimeUtc
        //            //InventorySystemId
        //            //NextDeliveryDateTimeUtc
        //            //UnitOfMeasurementSystemId
        //        });
        //        return this;
        //    }

        //    inventoryItem.InStockQuantity = inStockQuantity;

        //    return this;
        //}

        /* TODO:
         * BundledVariants
         * BundleOfVariants
         * Fields
         * RelationshipLinks
         * SortIndex
         * ChannelLinks
         */
    }
}
