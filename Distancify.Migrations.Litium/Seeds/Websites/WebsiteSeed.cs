using Litium;
using Litium.FieldFramework;
using LW = Litium.Websites;
using System;
using System.Text;

namespace Distancify.Migrations.Litium.Seeds.Websites
{
    public class WebsiteSeed : ISeed, ISeedGenerator<SeedBuilder.LitiumGraphQlModel.Website>
    {
        private LW.Website website;
        private string fieldTemplateId;

        private WebsiteSeed(LW.Website website, string fieldTemplateId)
        {
            this.website = website;
            this.fieldTemplateId = fieldTemplateId;
        }

        public void Commit()
        {
            var service = IoC.Resolve<LW.WebsiteService>();

            var websiteFieldTemplateSystemId = IoC.Resolve<FieldTemplateService>().Get<LW.WebsiteFieldTemplate>(fieldTemplateId).SystemId;
            website.FieldTemplateSystemId = websiteFieldTemplateSystemId;

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

            var websiteClone = IoC.Resolve<LW.WebsiteService>().Get(websiteName)?.MakeWritableClone();
            if (websiteClone is null)
            {
                websiteClone = new LW.Website(Guid.Empty);
                websiteClone.Id = websiteName;
                websiteClone.SystemId = Guid.Empty;
                websiteClone.Localizations["en-US"].Name = websiteName;
            }

            return new WebsiteSeed(websiteClone, websiteTemplateName);
        }

        public ISeedGenerator<SeedBuilder.LitiumGraphQlModel.Website> Update(SeedBuilder.LitiumGraphQlModel.Website data)
        {
            this.website.Id = data.Id;
            this.website.FieldTemplateSystemId = Guid.Empty;
            this.fieldTemplateId = data.FieldTemplate.Id;
            return this;
        }

        public void WriteMigration(StringBuilder builder)
        {
            builder.AppendLine($"\t\t\t{nameof(WebsiteSeed)}.{nameof(WebsiteSeed.Ensure)}(\"{website.Id}\",\"{fieldTemplateId}\")");
            builder.AppendLine("\t\t\t\t.Commit();");
        }

        internal static WebsiteSeed CreateFrom(SeedBuilder.LitiumGraphQlModel.Website website)
        {
            var seed = new WebsiteSeed(new LW.Website(Guid.Empty), string.Empty);
            return (WebsiteSeed)seed.Update(website);
        }
    }
}
