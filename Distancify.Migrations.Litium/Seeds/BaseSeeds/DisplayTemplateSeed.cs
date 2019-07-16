using Litium;
using Litium.Products;
using System;
using System.Linq;

namespace Distancify.Migrations.Litium.Seeds.BaseSeeds
{
    class DisplayTemplateSeed<T> : ISeed
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

        public DisplayTemplateSeed<T> WithName(string culture, string name)
        {
            if (!displayTemplate.Localizations.Any(l => l.Key.Equals(culture)) ||
                !displayTemplate.Localizations[culture].Name.Equals(name))
            {
                displayTemplate.Localizations[culture].Name = name;
            }

            return this;
        }
    }
}
