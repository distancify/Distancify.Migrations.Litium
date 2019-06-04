﻿using Litium;
using Litium.FieldFramework;
using Litium.Globalization;
using Litium.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distancify.Migrations.Litium.Products
{
    public class BaseProductSeed : ISeed
    {

        private readonly BaseProduct baseProduct;

        protected BaseProductSeed(BaseProduct baseProduct)
        {
            this.baseProduct = baseProduct;
        }

        public static BaseProductSeed Ensure(string productId, string productFieldTemplateId)
        {
            var fieldTemplate = IoC.Resolve<FieldTemplateService>().Get<ProductFieldTemplate>(productFieldTemplateId).SystemId;
            var productClone = IoC.Resolve<BaseProductService>().Get(productId)?.MakeWritableClone() ??
                new BaseProduct(productId, fieldTemplate)
                {
                    SystemId = Guid.Empty,
                    
                };

            return new BaseProductSeed(productClone);
        }

        public void Commit()
        {
            var service = IoC.Resolve<BaseProductService>();

            if (baseProduct.SystemId == null || baseProduct.SystemId == Guid.Empty)
            {
                baseProduct.SystemId = Guid.NewGuid();
                service.Create(baseProduct);
                return;
            }

            service.Update(baseProduct);
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

        /*
         * TODO: CategoryLinks
         * TODO: Fields
         * TODO: ProductListLinks
         * TODO: RelationshipLinks
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
        */
    }
}
