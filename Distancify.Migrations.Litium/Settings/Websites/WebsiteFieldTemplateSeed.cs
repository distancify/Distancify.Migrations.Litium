using Distancify.Migrations.Litium.BaseSeeds;
using Litium;
using Litium.FieldFramework;
using Litium.Websites;
using System;

namespace Distancify.Migrations.Litium.Settings.Websites
{
    public class WebsiteFieldTemplateSeed : FieldTemplateSeed
    {
        public WebsiteFieldTemplateSeed(WebsiteFieldTemplate fieldTemplate) : base(fieldTemplate)
        {
        }

        public static WebsiteFieldTemplateSeed Ensure(string pageFieldTemplateId)
        {
            var websiteFieldTemplate = (WebsiteFieldTemplate)IoC.Resolve<FieldTemplateService>().Get<WebsiteFieldTemplate>(pageFieldTemplateId)?.MakeWritableClone();
            if (websiteFieldTemplate is null)
            {
                websiteFieldTemplate = new WebsiteFieldTemplate(pageFieldTemplateId);
                websiteFieldTemplate.SystemId = Guid.Empty;
            }
            return new WebsiteFieldTemplateSeed(websiteFieldTemplate);
        }

        //TODO: areatype
    }
}
