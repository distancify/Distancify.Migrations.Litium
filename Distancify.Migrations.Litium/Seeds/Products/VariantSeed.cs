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
        private readonly Guid baseProductSystemId;
        private readonly bool _isNewVariant;
        private ISet<string> categoryLinks = new HashSet<string>();
        private string mainCategory;
        private List<PriceListItem> priceListItems = new List<PriceListItem>();

        protected VariantSeed(Variant variant, Guid baseProductSystemId, bool isNewVariant = false)
        {
            _variant = variant;
            this.baseProductSystemId = baseProductSystemId;
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


            if (categoryLinks.Count > 0)
            {
                var categoryService = IoC.Resolve<CategoryService>();
                
                var baseProduct = IoC.Resolve<BaseProductService>().Get(baseProductSystemId);

                foreach (var categoryId in categoryLinks)
                {
                    var category = categoryService.Get(categoryId).MakeWritableClone();
                    var categoryToProductLink = category.ProductLinks.FirstOrDefault(c => c.BaseProductSystemId == baseProduct.SystemId);

                    if (categoryToProductLink != null && !categoryToProductLink.ActiveVariantSystemIds.Contains(_variant.SystemId))
                    {
                        categoryToProductLink.ActiveVariantSystemIds.Add(_variant.SystemId);
                        categoryToProductLink.MainCategory = categoryId == mainCategory;
                    }
                    else if (categoryToProductLink == null)
                    {
                        category.ProductLinks.Add(new CategoryToProductLink(baseProductSystemId)
                        {
                            ActiveVariantSystemIds = new HashSet<Guid> { _variant.SystemId },
                            MainCategory = categoryId == mainCategory
                        });
                    }

                    categoryService.Update(category);
                }
            }

            if(priceListItems.Count > 0)
            {
                var priceListItemService = IoC.Resolve<PriceListItemService>();
                foreach(var priceItem in priceListItems)
                {
                    var existingPriceItem = priceListItemService.Get(priceItem.VariantSystemId, priceItem.PriceListSystemId)
                        ?.FirstOrDefault(p => p.MinimumQuantity == priceItem.MinimumQuantity)
                        ?.MakeWritableClone();

                    if(existingPriceItem == null)
                    {
                        priceListItemService.Create(priceItem);
                    }
                    else
                    {
                        existingPriceItem.MinimumQuantity = priceItem.MinimumQuantity;
                        existingPriceItem.Price = priceItem.Price;

                        priceListItemService.Update(existingPriceItem);
                    }
                }
            }

            return _variant.SystemId;
        }

        public static VariantSeed Ensure(string variantId, string baseProductId)
        {
            var baseProduct = IoC.Resolve<BaseProductService>().Get(baseProductId);
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

            return new VariantSeed(variantClone, baseProduct.SystemId, isNewVariant);
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

        public VariantSeed WithField(string fieldName, string culture, object value)
        {
            _variant.Fields.AddOrUpdateValue(fieldName, culture, value);

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

            var priceListItem = priceListItems.FirstOrDefault(p => p.PriceListSystemId == priceListSystemGuid && p.MinimumQuantity == minimumQuantity);
            if (priceListItem == null)
            {
                priceListItems.Add(new PriceListItem(_variant.SystemId, priceListSystemGuid)
                {
                    MinimumQuantity = minimumQuantity,
                    Price = price
                });
                return this;
            }

            priceListItem.Price = price;
            priceListItem.MinimumQuantity = minimumQuantity;

            return this;
        }

        public VariantSeed WithChannelLink(string channelId)
        {
            var channelService = IoC.Resolve<ChannelService>();
            return WithChannelLink(channelService.Get(channelId).SystemId);
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
            var fileSystemId = IoC.Resolve<FileService>().Get(fileId).SystemId;

            return WithImage(fileSystemId);
        }

        public VariantSeed WithImage(Guid fileSystemId)
        {
            var images = _variant.Fields.GetValue<IList<Guid>>(SystemFieldDefinitionConstants.Images) ?? new List<Guid>();
            
            if (!images.Contains(fileSystemId))
            {
                images.Add(fileSystemId);
                _variant.Fields.AddOrUpdateValue(SystemFieldDefinitionConstants.Images, images);
            }

            return this;
        }

        public VariantSeed WithCategoryLink(string categoryId)
        {
            categoryLinks.Add(categoryId);

            return this;
        }

        public VariantSeed WithMainCategoryLink(string categoryId)
        {
            mainCategory = categoryId;

            return WithCategoryLink(categoryId);
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
