using Litium;
using Litium.FieldFramework;
using Litium.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distancify.Migrations.Litium.Products
{
    public class AssortmentCategorySeed : ISeed
    {
        private readonly Category category;

        protected AssortmentCategorySeed(Category category)
        {
            this.category = category;
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

            if (category.SystemId == null || category.SystemId == Guid.Empty)
            {
                category.SystemId = Guid.NewGuid();
                service.Create(category);
                return;
            }

            service.Update(category);
        }

        public AssortmentCategorySeed WithName(string culture, string name)
        {
            if (!category.Localizations.Any(l => l.Key.Equals(culture)) ||
                string.IsNullOrEmpty(category.Localizations[culture].Name) ||
                !category.Localizations[culture].Name.Equals(name))
            {
                category.Localizations[culture].Name = name;
            }

            return this;
        }

        public AssortmentCategorySeed WithDescription(string culture, string description)
        {
            if (!category.Localizations.Any(l => l.Key.Equals(culture)) ||
                string.IsNullOrEmpty(category.Localizations[culture].Description) ||
                !category.Localizations[culture].Description.Equals(description))
            {
                category.Localizations[culture].Description = description;
            }

            return this;
        }

        public AssortmentCategorySeed WithSeoDescription(string culture, string seoDescription)
        {
            if (!category.Localizations.Any(l => l.Key.Equals(culture)) ||
                string.IsNullOrEmpty(category.Localizations[culture].SeoDescription) ||
                !category.Localizations[culture].SeoDescription.Equals(seoDescription))
            {
                category.Localizations[culture].SeoDescription = seoDescription;
            }

            return this;
        }

        public AssortmentCategorySeed WithSeoTitle(string culture, string seoTitle)
        {
            if (!category.Localizations.Any(l => l.Key.Equals(culture)) ||
                string.IsNullOrEmpty(category.Localizations[culture].SeoTitle) ||
                !category.Localizations[culture].SeoTitle.Equals(seoTitle))
            {
                category.Localizations[culture].SeoTitle = seoTitle;
            }

            return this;
        }

        public AssortmentCategorySeed WithUrl(string culture, string url)
        {
            if (!category.Localizations.Any(l => l.Key.Equals(culture)) ||
                string.IsNullOrEmpty(category.Localizations[culture].Url) ||
                !category.Localizations[culture].Url.Equals(url))
            {
                category.Localizations[culture].Url = url;
            }

            return this;
        }

        public string GenerateMigration()
        {
            throw new NotImplementedException();
        }
    }
}
