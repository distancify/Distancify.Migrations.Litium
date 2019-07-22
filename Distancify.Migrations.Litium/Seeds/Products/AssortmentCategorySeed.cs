using System;
using System.Collections.Generic;
using System.Linq;
using Litium;
using Litium.FieldFramework;
using Litium.Globalization;
using Litium.Products;

namespace Distancify.Migrations.Litium.Seeds.Products
{
    public class AssortmentCategorySeed : ISeed
    {
        private readonly Category _category;

        protected AssortmentCategorySeed(Category category)
        {
            this._category = category;
        }

        public static AssortmentCategorySeed Ensure(string assortmentCategoryId, string categoryFieldTemplateId, string assortmentId)
        {
            var fieldTemplateSystemGuid = IoC.Resolve<FieldTemplateService>().Get<CategoryFieldTemplate>(categoryFieldTemplateId).SystemId;
            var assortmentSystemGuid = IoC.Resolve<AssortmentService>().Get(assortmentId).SystemId;
            var categoryClone = IoC.Resolve<CategoryService>().Get(assortmentCategoryId)?.MakeWritableClone() ??
                new Category(fieldTemplateSystemGuid, assortmentSystemGuid)
                {
                    SystemId = Guid.Empty,
                    Id = assortmentCategoryId
                };

            return new AssortmentCategorySeed(categoryClone);
        }

        public void Commit()
        {
            var service = IoC.Resolve<CategoryService>();

            if (_category.SystemId == null || _category.SystemId == Guid.Empty)
            {
                _category.SystemId = Guid.NewGuid();
                service.Create(_category);
                return;
            }

            service.Update(_category);
        }

        public AssortmentCategorySeed WithName(string culture, string name)
        {
            if (!_category.Localizations.Any(l => l.Key.Equals(culture)) ||
                string.IsNullOrEmpty(_category.Localizations[culture].Name) ||
                !_category.Localizations[culture].Name.Equals(name))
            {
                _category.Localizations[culture].Name = name;
            }

            return this;
        }

        public AssortmentCategorySeed WithDescription(string culture, string description)
        {
            if (!_category.Localizations.Any(l => l.Key.Equals(culture)) ||
                string.IsNullOrEmpty(_category.Localizations[culture].Description) ||
                !_category.Localizations[culture].Description.Equals(description))
            {
                _category.Localizations[culture].Description = description;
            }

            return this;
        }

        public AssortmentCategorySeed WithSeoDescription(string culture, string seoDescription)
        {
            if (!_category.Localizations.Any(l => l.Key.Equals(culture)) ||
                string.IsNullOrEmpty(_category.Localizations[culture].SeoDescription) ||
                !_category.Localizations[culture].SeoDescription.Equals(seoDescription))
            {
                _category.Localizations[culture].SeoDescription = seoDescription;
            }

            return this;
        }

        public AssortmentCategorySeed WithSeoTitle(string culture, string seoTitle)
        {
            if (!_category.Localizations.Any(l => l.Key.Equals(culture)) ||
                string.IsNullOrEmpty(_category.Localizations[culture].SeoTitle) ||
                !_category.Localizations[culture].SeoTitle.Equals(seoTitle))
            {
                _category.Localizations[culture].SeoTitle = seoTitle;
            }

            return this;
        }

        public AssortmentCategorySeed WithUrl(string culture, string url)
        {
            if (!_category.Localizations.Any(l => l.Key.Equals(culture)) ||
                string.IsNullOrEmpty(_category.Localizations[culture].Url) ||
                !_category.Localizations[culture].Url.Equals(url))
            {
                _category.Localizations[culture].Url = url;
            }

            return this;
        }

        public AssortmentCategorySeed WithChannelLink(string channelId)
        {
            var channelSystemId = IoC.Resolve<ChannelService>().Get(channelId).SystemId;

            if (_category.ChannelLinks is null)
            {
                _category.ChannelLinks = new List<CategoryToChannelLink>();
            }

            if (!_category.ChannelLinks.Any(cl => cl.ChannelSystemId == channelSystemId))
            {
                _category.ChannelLinks.Add(new CategoryToChannelLink(channelSystemId));
            }

            return this;
        }

        public AssortmentCategorySeed WithParentCategory(string categoryId)
        {
            _category.ParentCategorySystemId = IoC.Resolve<CategoryService>().Get(categoryId).SystemId;

            return this;
        }
    }
}
