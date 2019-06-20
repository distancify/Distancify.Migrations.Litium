using Litium;
using Litium.FieldFramework;
using Litium.Websites;
using System;
using System.Text;
using Graphql = Distancify.Migrations.Litium.LitiumGraphqlModel;

namespace Distancify.Migrations.Litium.Websites
{
    public class WebsiteSeed : ISeed
    {
        private Website website;
        private Graphql.Website graphqlWebsite;

        public WebsiteSeed(Graphql.Website graphqlWebsite)
        {
            this.graphqlWebsite = graphqlWebsite;
        }

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

        public string GenerateMigration()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine($"\t\t\t{nameof(WebsiteSeed)}.{nameof(WebsiteSeed.Ensure)}(\"{graphqlWebsite.Id}\",\"{graphqlWebsite.FieldTemplate.Id}\")");
            builder.AppendLine("\t\t\t\t.Commit();");
            return builder.ToString();
        }
    }
}
