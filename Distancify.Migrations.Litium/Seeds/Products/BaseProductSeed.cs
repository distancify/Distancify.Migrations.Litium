using System;
using System.Collections.Generic;
using System.Linq;
using Litium;
using Litium.FieldFramework;
using Litium.Globalization;
using Litium.Media;
using Litium.Products;
using Litium.Customers;
using Litium.Security;

namespace Distancify.Migrations.Litium.Seeds.Products
{
    public class BaseProductSeed : ISeed
    {

        private readonly BaseProduct baseProduct;
        private ISet<string> categoryLinks = new HashSet<string>();
        private string mainCategory;

        private readonly bool _isNewBaseProduct = false;

        protected BaseProductSeed(BaseProduct baseProduct, bool isNewBaseProduct = false)
        {
            this.baseProduct = baseProduct;
            _isNewBaseProduct = isNewBaseProduct;
        }

        public static BaseProductSeed Ensure(string productId, string productFieldTemplateId)
        {
            var isNewBaseProduct = false;

            var fieldTemplate = IoC.Resolve<FieldTemplateService>().Get<ProductFieldTemplate>(productFieldTemplateId).SystemId;
            var productClone = IoC.Resolve<BaseProductService>().Get(productId)?.MakeWritableClone();
            
            if (productClone is null)
            {
                productClone = new BaseProduct(productId, fieldTemplate)
                {
                    SystemId = Guid.NewGuid(),
                };

                isNewBaseProduct = true;
            }

            return new BaseProductSeed(productClone, isNewBaseProduct);
        }

        public Guid Commit()
        {
            var service = IoC.Resolve<BaseProductService>();

            if (_isNewBaseProduct)
            {
                service.Create(baseProduct);
            }
            else
            {
                service.Update(baseProduct);
            }

            if (categoryLinks.Count > 0)
            {
                var categoryService = IoC.Resolve<CategoryService>();
                foreach (var categoryLink in categoryLinks)
                {
                    var category = categoryService.Get(categoryLink)?.MakeWritableClone();

                    var productLink = category.ProductLinks.FirstOrDefault(l => l.BaseProductSystemId == baseProduct.SystemId);
                    if (productLink == null)
                    {
                        category.ProductLinks.Add(new CategoryToProductLink(baseProduct.SystemId)
                        {
                            MainCategory = categoryLink == mainCategory
                        });
                    }
                    else
                    {
                        productLink.MainCategory = categoryLink == mainCategory;
                        categoryService.Update(category);
                    }
                }
            }

            return baseProduct.SystemId;
        }

        public BaseProductSeed WithVisitorReadPermission()
        {
            var visitorGroupSystemId = IoC.Resolve<GroupService>().Get<StaticGroup>(LitiumMigration.SystemConstants.Visitors).SystemId;

            if (!baseProduct.AccessControlList.Any(a => a.GroupSystemId == visitorGroupSystemId))
            {
                baseProduct.AccessControlList.Add(new AccessControlEntry(Operations.Entity.Read, visitorGroupSystemId));
            }

            return this;
        }

        // Active is obsolutlete by Litium

        public BaseProductSeed WithTaxClassSystem(string taxClassId)
        {
            var taxClassSystemGuid = IoC.Resolve<TaxClassService>().Get(taxClassId).SystemId;
            baseProduct.TaxClassSystemId = taxClassSystemGuid;
            return this;
        }

        public BaseProductSeed WithName(string culture, string name)
        {
            if (!baseProduct.Localizations.Any(l => l.Key.Equals(culture)) ||
                string.IsNullOrEmpty(baseProduct.Localizations[culture].Name) ||
                !baseProduct.Localizations[culture].Name.Equals(name))
            {
                baseProduct.Localizations[culture].Name = name;
            }

            return this;
        }

        public BaseProductSeed WithDescription(string culture, string description)
        {
            if (!baseProduct.Localizations.Any(l => l.Key.Equals(culture)) ||
                string.IsNullOrEmpty(baseProduct.Localizations[culture].Description) ||
                !baseProduct.Localizations[culture].Description.Equals(description))
            {
                baseProduct.Localizations[culture].Description = description;
            }

            return this;
        }

        public BaseProductSeed WithSeoDescription(string culture, string seoDescription)
        {
            if (!baseProduct.Localizations.Any(l => l.Key.Equals(culture)) ||
                string.IsNullOrEmpty(baseProduct.Localizations[culture].SeoDescription) ||
                !baseProduct.Localizations[culture].SeoDescription.Equals(seoDescription))
            {
                baseProduct.Localizations[culture].SeoDescription = seoDescription;
            }

            return this;
        }

        public BaseProductSeed WithSeoTitle(string culture, string seoTitle)
        {
            if (!baseProduct.Localizations.Any(l => l.Key.Equals(culture)) ||
                string.IsNullOrEmpty(baseProduct.Localizations[culture].SeoTitle) ||
                !baseProduct.Localizations[culture].SeoTitle.Equals(seoTitle))
            {
                baseProduct.Localizations[culture].SeoTitle = seoTitle;
            }

            return this;
        }

        public BaseProductSeed WithUrl(string culture, string url)
        {
            if (!baseProduct.Localizations.Any(l => l.Key.Equals(culture)) ||
                string.IsNullOrEmpty(baseProduct.Localizations[culture].Url) ||
                !baseProduct.Localizations[culture].Url.Equals(url))
            {
                baseProduct.Localizations[culture].Url = url;
            }

            return this;
        }

        public BaseProductSeed WithCategoryLink(string assortmentCategoryId)
        {
            categoryLinks.Add(assortmentCategoryId);

            return this;
        }

        public BaseProductSeed WithMainCategoryLink(string assortmentCategoryId)
        {
            mainCategory = assortmentCategoryId;
            return WithCategoryLink(assortmentCategoryId);
        }

        public BaseProductSeed WithField(string fieldName, Dictionary<string, object> values)
        {
            foreach (var localization in values.Keys)
            {
                baseProduct.Fields.AddOrUpdateValue(fieldName, localization, values[localization]);
            }

            return this;
        }

        public BaseProductSeed WithField(string fieldName, object value)
        {
            baseProduct.Fields.AddOrUpdateValue(fieldName, value);

            return this;
        }

        public BaseProductSeed WithField(string fieldName, string culture, object value)
        {
            baseProduct.Fields.AddOrUpdateValue(fieldName, culture, value);

            return this;
        }

        public BaseProductSeed WithImage(string fileId)
        {
            var images = baseProduct.Fields.GetValue<IList<Guid>>(SystemFieldDefinitionConstants.Images) ?? new List<Guid>();
            var fileSystemId = IoC.Resolve<FileService>().Get(fileId).SystemId;

            if (!images.Contains(fileSystemId))
            {
                images.Add(fileSystemId);
                baseProduct.Fields.AddOrUpdateValue(SystemFieldDefinitionConstants.Images, images);
            }

            return this;
        }

        public BaseProductSeed WithBaseProductRelation(string relationshipTypeId, string relatedBaseProductId)
        {
            var relationshipTypeService = IoC.Resolve<RelationshipTypeService>();
            var relationshipType = relationshipTypeService.Get(relationshipTypeId);

            var baseProductService = IoC.Resolve<BaseProductService>();
            var relatedBaseProduct = baseProductService.Get(relatedBaseProductId);

            baseProduct.RelationshipLinks.Add(new BaseProductToBaseProductRelationshipLink(relationshipType.SystemId, relatedBaseProduct.SystemId));

            return this;
        }

        public BaseProductSeed WithVariantRelation(string relationshipTypeId, string relatedVariantId)
        {
            var relationshipTypeService = IoC.Resolve<RelationshipTypeService>();
            var relationshipType = relationshipTypeService.Get(relationshipTypeId);

            var variantService = IoC.Resolve<VariantService>();
            var relatedVariant = variantService.Get(relatedVariantId);

            baseProduct.RelationshipLinks.Add(new BaseProductToVariantRelationshipLink(relationshipType.SystemId, relatedVariant.SystemId));

            return this;
        }


        /// <summary>
        /// Sets the future system id of new entities.
        /// </summary>
        public BaseProductSeed WithSystemId(Guid systemId)
        {
            baseProduct.SystemId = systemId;

            return this;
        }

        /*
* TODO: Remove CategoryLinks
* TODO: Fields
* TODO: ProductListLinks
*/

            /*TODO:?
             With
            - Image
            - Variants
            - Relations
            - Bundle
            - Plan
            - Publish
            - Price (variant?)
            - Inventory
            - Workflow
            - History
            - Settings

            TaxClass?
            */
    }
}
