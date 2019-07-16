using Distancify.Migrations.Litium.Seeds.BaseSeeds;
using Litium;
using Litium.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Distancify.Migrations.Litium.Seeds.Product
{
    public class ProductDisplayTemplateSeed : DisplayTemplateSeed<ProductDisplayTemplate>, ISeedGenerator<SeedBuilder.LitiumGraphQlModel.Products.ProductDisplayTemplate>
    {

        public ProductDisplayTemplateSeed(ProductDisplayTemplate productDisplayTemplate)
            :base(productDisplayTemplate)
        {

        }

        public static ProductDisplayTemplateSeed Ensure(string id)
        {
            var displayTemplateClone = IoC.Resolve<DisplayTemplateService>().Get<ProductDisplayTemplate>(id)?.MakeWritableClone();
            if (displayTemplateClone is null)
            {
                displayTemplateClone = new ProductDisplayTemplate
                {
                    Id = id,
                    SystemId = Guid.Empty
                };
            }

            return new ProductDisplayTemplateSeed(displayTemplateClone);
        }

        public static ProductDisplayTemplateSeed CreateFrom(SeedBuilder.LitiumGraphQlModel.Products.ProductDisplayTemplate productDisplayTemplate)
        {
            var seed = new ProductDisplayTemplateSeed(new ProductDisplayTemplate());
            return (ProductDisplayTemplateSeed)seed.Update(productDisplayTemplate);
        }

        public ISeedGenerator<SeedBuilder.LitiumGraphQlModel.Products.ProductDisplayTemplate> Update(SeedBuilder.LitiumGraphQlModel.Products.ProductDisplayTemplate data)
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

            builder.AppendLine($"\r\n\t\t\t{nameof(ProductDisplayTemplateSeed)}.{nameof(ProductDisplayTemplateSeed.Ensure)}(\"{displayTemplate.Id}\")");

            foreach (var localization in displayTemplate.Localizations)
            {
                builder.AppendLine($"\t\t\t\t.{nameof(WithName)}(\"{localization.Key}\", \"{localization.Value.Name}\")");
            }

            WritePropertiesMigration(builder);

            builder.AppendLine("\t\t\t\t.Commit();");
        }
    }
}
