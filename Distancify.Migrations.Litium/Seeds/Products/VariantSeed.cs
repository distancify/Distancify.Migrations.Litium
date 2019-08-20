using System;
using System.Collections.Generic;
using System.Linq;
using Litium;
using Litium.FieldFramework;
using Litium.Globalization;
using Litium.Media;
using Litium.Products;

namespace Distancify.Migrations.Litium.Seeds.Products
{
    public class VariantSeed : ISeed
    {
        private readonly Variant _variant;
        private readonly BaseProduct _baseProduct;
        private readonly bool _isNewVariant;
        private bool _baseProductUpdated = false;

        protected VariantSeed(Variant variant, BaseProduct baseProduct, bool isNewVariant = false)
        {
            _variant = variant;
            _baseProduct = baseProduct;
            _isNewVariant = isNewVariant;
        }

        public Guid Commit()
        {
            var service = IoC.Resolve<VariantService>();

            if (_isNewVariant)
            {
                service.Create(_variant);
            }
            else
            {
                service.Update(_variant);
            }


            if (_baseProductUpdated)
            {
                IoC.Resolve<BaseProductService>().Update(_baseProduct);
            }

            return _variant.SystemId;
        }

        public static VariantSeed Ensure(string variantId, string baseProductId)
        {
            var baseProduct = IoC.Resolve<BaseProductService>().Get(baseProductId).MakeWritableClone();
            var variantClone = IoC.Resolve<VariantService>().Get(variantId)?.MakeWritableClone();
            var isNewVariant = false;

            if (variantClone is null)
            {
                variantClone = new Variant(variantId, baseProduct.SystemId)
                {
                    SystemId = Guid.NewGuid()
                };
                isNewVariant = true;
            }

            return new VariantSeed(variantClone, baseProduct, isNewVariant);
        }
        public VariantSeed WithName(string culture, string name)
        {
            if (!_variant.Localizations.Any(l => l.Key.Equals(culture)) ||
                string.IsNullOrEmpty(_variant.Localizations[culture].Name) ||
                !_variant.Localizations[culture].Name.Equals(name))
            {
                _variant.Localizations[culture].Name = name;
            }

            return this;
        }

        public VariantSeed WithField(string fieldName, Dictionary<string, object> values)
        {
            foreach (var localization in values.Keys)
            {
                _variant.Fields.AddOrUpdateValue(fieldName, localization, values[localization]);
            }

            return this;
        }

        public VariantSeed WithField(string fieldName, object value)
        {
            _variant.Fields.AddOrUpdateValue(fieldName, value);

            return this;
        }

        public VariantSeed WithDescription(string culture, string description)
        {
            if (!_variant.Localizations.Any(l => l.Key.Equals(culture)) ||
                string.IsNullOrEmpty(_variant.Localizations[culture].Description) ||
                !_variant.Localizations[culture].Description.Equals(description))
            {
                _variant.Localizations[culture].Description = description;
            }

            return this;
        }

        public VariantSeed WithSeoDescription(string culture, string seoDescription)
        {
            if (!_variant.Localizations.Any(l => l.Key.Equals(culture)) ||
                string.IsNullOrEmpty(_variant.Localizations[culture].SeoDescription) ||
                !_variant.Localizations[culture].SeoDescription.Equals(seoDescription))
            {
                _variant.Localizations[culture].SeoDescription = seoDescription;
            }

            return this;
        }

        public VariantSeed WithSeoTitle(string culture, string seoTitle)
        {
            if (!_variant.Localizations.Any(l => l.Key.Equals(culture)) ||
                string.IsNullOrEmpty(_variant.Localizations[culture].SeoTitle) ||
                !_variant.Localizations[culture].SeoTitle.Equals(seoTitle))
            {
                _variant.Localizations[culture].SeoTitle = seoTitle;
            }

            return this;
        }

        public VariantSeed WithUrl(string culture, string url)
        {
            if (!_variant.Localizations.Any(l => l.Key.Equals(culture)) ||
                string.IsNullOrEmpty(_variant.Localizations[culture].Url) ||
                !_variant.Localizations[culture].Url.Equals(url))
            {
                _variant.Localizations[culture].Url = url;
            }

            return this;
        }

        public VariantSeed WithPrice(string priceListId, decimal price)
        {
            return WithPrice(priceListId, price, default);
        }

        public VariantSeed WithPrice(string priceListId, decimal price, decimal minimumQuantity)
        {
            var priceListSystemGuid = IoC.Resolve<PriceListService>().Get(priceListId).SystemId;
            var priceItem = _variant.Prices.FirstOrDefault(p => p.PriceListSystemId == priceListSystemGuid && p.MinimumQuantity == minimumQuantity);

            if (priceItem == null)
            {
                _variant.Prices.Add(
                    new VariantPriceItem(priceListSystemGuid)
                    {
                        MinimumQuantity = minimumQuantity,
                        Price = price
                        //VatPercentage
                    });
                return this;
            }

            priceItem.Price = price;
            priceItem.MinimumQuantity = minimumQuantity;

            return this;
        }

        public VariantSeed WithChannelLink(string channelId)
        {
            var channelService = IoC.Resolve<ChannelService>();
            return WithChannelLink(channelService.Get(channelId).SystemId);

            if (channelService.Get(channelId) is Channel channel)
            {
                return WithChannelLink(channel.SystemId);
            }

            return this;
        }

        public VariantSeed WithChannelLink(Guid channelSystemId)
        {
            if (!_variant.ChannelLinks.Any(l => l.ChannelSystemId == channelSystemId))
            {
                _variant.ChannelLinks.Add(new VariantToChannelLink(channelSystemId));
            }

            return this;
        }

        public VariantSeed WithUnitOfMeasurement(string unitOfMeasurementId)
        {
            var unitOfMeasurementSystemId = IoC.Resolve<UnitOfMeasurementService>().Get(unitOfMeasurementId).SystemId;

            _variant.UnitOfMeasurementSystemId = unitOfMeasurementSystemId;
            return this;
        }

        public VariantSeed WithImage(string fileId)
        {
            var images = _variant.Fields.GetValue<IList<Guid>>(SystemFieldDefinitionConstants.Images) ?? new List<Guid>();
            var fileSystemId = IoC.Resolve<FileService>().Get(fileId).SystemId;

            if (!images.Contains(fileSystemId))
            {
                images.Add(fileSystemId);
                _variant.Fields.AddOrUpdateValue(SystemFieldDefinitionConstants.Images, images);
            }

            return this;
        }

        public VariantSeed WithCategoryLink(string categoryId)
        {
            var categorySystemId = IoC.Resolve<CategoryService>().Get(categoryId).SystemId;
            var baseProductToCategoryLink = _baseProduct.CategoryLinks.FirstOrDefault(c => c.CategorySystemId == categorySystemId);

            if (baseProductToCategoryLink != null && !baseProductToCategoryLink.ActiveVariantSystemIds.Contains(_variant.SystemId))
            {
                baseProductToCategoryLink.ActiveVariantSystemIds.Add(_variant.SystemId);
                _baseProductUpdated = true;
            }
            else if (baseProductToCategoryLink == null)
            {
                _baseProduct.CategoryLinks.Add(new BaseProductToCategoryLink(categorySystemId)
                {
                    ActiveVariantSystemIds = new HashSet<Guid> { _variant.SystemId }
                });
                _baseProductUpdated = true;
            }

            return this;
        }

        public VariantSeed WithBaseProductRelation(string relationshipTypeId, string relatedBaseProductId)
        {
            var relationshipTypeService = IoC.Resolve<RelationshipTypeService>();
            var relationshipType = relationshipTypeService.Get(relationshipTypeId);

            var baseProductService = IoC.Resolve<BaseProductService>();
            var relatedBaseProduct = baseProductService.Get(relatedBaseProductId);

            _variant.RelationshipLinks.Add(new VariantToBaseProductRelationshipLink(relationshipType.SystemId, relatedBaseProduct.SystemId));

            return this;
        }

        public VariantSeed WithVariantRelation(string relationshipTypeId, string relatedVariantId)
        {
            var relationshipTypeService = IoC.Resolve<RelationshipTypeService>();
            var relationshipType = relationshipTypeService.Get(relationshipTypeId);

            var variantService = IoC.Resolve<VariantService>();
            var relatedVariant = variantService.Get(relatedVariantId);

            _variant.RelationshipLinks.Add(new VariantToVariantRelationshipLink(relationshipType.SystemId, relatedVariant.SystemId));

            return this;
        }

        /* TODO:
         * BundledVariants
         * BundleOfVariants
         * SortIndex
         */
    }
}
