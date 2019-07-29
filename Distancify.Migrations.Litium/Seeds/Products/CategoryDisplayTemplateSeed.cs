using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distancify.Migrations.Litium.Seeds.FieldFramework;
using Litium;
using Litium.Products;

namespace Distancify.Migrations.Litium.Seeds.Products
{
    public class CategoryDisplayTemplateSeed : DisplayTemplateSeed<CategoryDisplayTemplate>, ISeedGenerator<SeedBuilder.LitiumGraphQlModel.Products.CategoryDisplayTemplate>
    {
        public CategoryDisplayTemplateSeed(CategoryDisplayTemplate categoryDisplayTemplate) : base(categoryDisplayTemplate)
        {
        }

        public static CategoryDisplayTemplateSeed Ensure(string id)
        {
            var displayTemplateClone = IoC.Resolve<DisplayTemplateService>().Get<CategoryDisplayTemplate>(id)?.MakeWritableClone();
            if (displayTemplateClone is null)
            {
                displayTemplateClone = new CategoryDisplayTemplate
                {
                    Id = id,
                    SystemId = Guid.Empty
                };
            }

            return new CategoryDisplayTemplateSeed(displayTemplateClone);
        }

        public static CategoryDisplayTemplateSeed CreateFrom(SeedBuilder.LitiumGraphQlModel.Products.CategoryDisplayTemplate categoryDisplayTemplate)
        {
            var seed = new CategoryDisplayTemplateSeed(new CategoryDisplayTemplate());
            return (CategoryDisplayTemplateSeed)seed.Update(categoryDisplayTemplate);
        }

        public ISeedGenerator<SeedBuilder.LitiumGraphQlModel.Products.CategoryDisplayTemplate> Update(SeedBuilder.LitiumGraphQlModel.Products.CategoryDisplayTemplate data)
        {
            displayTemplate.Id = data.Id;
            displayTemplate.SystemId = data.SystemId;

            displayTemplate.TemplatePath = data.TemplatePath;
            displayTemplate.Templates = data.Templates?.Select(t => new DisplayTemplateToWebSiteLink { Path = t.Path, WebSiteSystemId = t.WebsiteSystemId }).ToList()
                ?? new List<DisplayTemplateToWebSiteLink>();

            foreach (var localization in data.Localizations)
            {
                if (!string.IsNullOrEmpty(localization.Culture) && !string.IsNullOrEmpty(localization.Name))
                {
                    displayTemplate.Localizations[localization.Culture].Name = localization.Name;
                }
                else
                {
                    this.Log().Warn("The Dsiplay Template with system id {DisplayTemplateSystemId} contains a localization with an empty culture and/or name!",
                        data.SystemId.ToString());
                }
            }

            return this;
        }

        public void WriteMigration(StringBuilder builder)
        {
            if (string.IsNullOrEmpty(displayTemplate.Id))
            {
                throw new Exception("Can't ensure Display Template With an empty/null id");
            }

            builder.AppendLine($"\r\n\t\t\t{nameof(CategoryDisplayTemplateSeed)}.{nameof(CategoryDisplayTemplateSeed.Ensure)}(\"{displayTemplate.Id}\")");

            foreach (var localization in displayTemplate.Localizations)
            {
                builder.AppendLine($"\t\t\t\t.{nameof(WithName)}(\"{localization.Key}\", \"{localization.Value.Name}\")");
            }

            WritePropertiesMigration(builder);

            builder.AppendLine("\t\t\t\t.Commit();");
        }
    }
}
