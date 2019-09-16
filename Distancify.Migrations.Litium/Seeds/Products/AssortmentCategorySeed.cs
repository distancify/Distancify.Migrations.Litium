using System;
using System.Collections.Generic;
using System.Linq;
using Litium;
using Litium.FieldFramework;
using Litium.Globalization;
using Litium.Products;
using FieldData = Distancify.Migrations.Litium.SeedBuilder.LitiumGraphQlModel.FieldData;

namespace Distancify.Migrations.Litium.Seeds.Products
{
    public class AssortmentCategorySeed : ISeed
    {
        private readonly Category _category;
        private Guid _newSystemId;

        protected AssortmentCategorySeed(Category category)
        {
            _category = category;
            _newSystemId = Guid.NewGuid(); 
        }

        public static AssortmentCategorySeed Ensure(string assortmentCategoryId, string categoryFieldTemplateId, string assortmentId)
        {
            var fieldTemplateService = IoC.Resolve<FieldTemplateService>();
            var fieldTemplate = fieldTemplateService.Get<CategoryFieldTemplate>(categoryFieldTemplateId);

            var assortmentService = IoC.Resolve<AssortmentService>();
            var assortment = assortmentService.Get(assortmentId);

            var categoryService = IoC.Resolve<CategoryService>();
            var category = categoryService.Get(assortmentCategoryId);

            var categoryClone = category?.MakeWritableClone() ??
                new Category(fieldTemplate.SystemId, assortment.SystemId)
                {
                    SystemId = Guid.Empty,
                    Id = assortmentCategoryId
                };

            return new AssortmentCategorySeed(categoryClone);
        }

        public Guid Commit()
        {
            var service = IoC.Resolve<CategoryService>();

            if (_category.SystemId == null || _category.SystemId == Guid.Empty)
            {
                _category.SystemId = _newSystemId;
                service.Create(_category);
            }
            else
            {
                service.Update(_category);
            }

            return _category.SystemId;
        }

        public AssortmentCategorySeed WithSystemId(Guid systemId)
        {
            _newSystemId = systemId;
            return this;
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
            return this.WithChannelLink(IoC.Resolve<ChannelService>().Get(channelId).SystemId);
        }

        public AssortmentCategorySeed WithChannelLink(Guid channelSystemId)
        {
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

        public AssortmentCategorySeed WithField(string fieldName, Dictionary<string, object> values)
        {
            foreach (var localization in values.Keys)
            {
                _category.Fields.AddOrUpdateValue(fieldName, localization, values[localization]);
            }

            return this;
        }

        public AssortmentCategorySeed WithField(string fieldName, object value)
        {
            _category.Fields.AddOrUpdateValue(fieldName, value);
            return this;
        }

        public AssortmentCategorySeed WithField(string fieldName, object value, string culture)
        {
            _category.Fields.AddOrUpdateValue(fieldName, culture, value);
            return this;
        }
    }
}
