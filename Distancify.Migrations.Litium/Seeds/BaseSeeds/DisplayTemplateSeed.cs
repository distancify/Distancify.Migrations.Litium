using Litium;
using Litium.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Distancify.Migrations.Litium.Seeds.BaseSeeds
{
    public abstract class DisplayTemplateSeed<T> : ISeed
        where T : DisplayTemplate
    {
        protected readonly T displayTemplate;

        protected DisplayTemplateSeed(T displayTemplate)
        {
            this.displayTemplate = displayTemplate;
        }

        public void Commit()
        {
            var service = IoC.Resolve<DisplayTemplateService>();

            if (displayTemplate.SystemId == null || displayTemplate.SystemId == Guid.Empty)
            {
                displayTemplate.SystemId = Guid.NewGuid();
                service.Create(displayTemplate);
                return;
            }
            service.Update(displayTemplate);
        }

        public DisplayTemplateSeed<T> WithTemplatePath(string templatePath)
        {
            displayTemplate.TemplatePath = templatePath;
            return this;
        }

        public DisplayTemplateSeed<T> WithTemplate(Guid websiteSystemId, string path)
        {
            if (displayTemplate.Templates == null)
            {
                displayTemplate.Templates = new List<DisplayTemplateToWebSiteLink>();
            }

            if (displayTemplate.Templates.FirstOrDefault(t => t.WebSiteSystemId == websiteSystemId) is DisplayTemplateToWebSiteLink link)
            {
                link.Path = path;
            }
            else
            {
                displayTemplate.Templates.Add(new DisplayTemplateToWebSiteLink() { Path = path, WebSiteSystemId = websiteSystemId });
            }

            return this;
        }

        public DisplayTemplateSeed<T> WithName(string culture, string name)
        {
            if (!displayTemplate.Localizations.Any(l => l.Key.Equals(culture)) ||
                !displayTemplate.Localizations[culture].Name.Equals(name))
            {
                displayTemplate.Localizations[culture].Name = name;
            }

            return this;
        }

        public void WritePropertiesMigration(StringBuilder builder)
        {
            builder.AppendLine($"\t\t\t\t.{nameof(WithTemplatePath)}(\"{displayTemplate.TemplatePath}\")");

            foreach (var template in displayTemplate.Templates)
            {
                builder.AppendLine($"\t\t\t\t.{nameof(WithTemplate)}(Guid.Parse(\"{template.WebSiteSystemId.ToString()}\"), \"{template.Path}\")");
            }
        }
    }
}
