using Litium;
using Litium.FieldFramework;
using Litium.Websites;
using System;

namespace Distancify.Migrations.Litium.Settings.Websites
{
    public class WebsiteSeed : ISeed
    {
        private Website website;


        protected WebsiteSeed(Website website)
        {
            this.website = website;
        }

        public void Commit()
        {
            var service = IoC.Resolve<WebsiteService>();

            if (website.SystemId == null || website.SystemId == Guid.Empty)
            {
                website.SystemId = Guid.NewGuid();
                service.Create(website);
                return;
            }

            service.Update(website);
        }

        public static WebsiteSeed Ensure(string websiteName, string websiteTemplateName)
        {
            var websiteFieldTemplateSystemId = IoC.Resolve<FieldTemplateService>().Get<WebsiteFieldTemplate>(websiteTemplateName).SystemId;

            var websiteClone = IoC.Resolve<WebsiteService>().Get(websiteName)?.MakeWritableClone();
            if (websiteClone is null)
            {
                websiteClone = new Website(Guid.Empty);
                websiteClone.Id = websiteName;
                websiteClone.SystemId = Guid.Empty;
                websiteClone.Localizations["en-US"].Name = websiteName;
                websiteClone.FieldTemplateSystemId = websiteFieldTemplateSystemId;
            }

            return new WebsiteSeed(websiteClone);
        }
    }
}
