using System;
using System.Text;
using Litium;
using Litium.FieldFramework;
using Litium.Websites;

namespace Distancify.Migrations.Litium.Seeds.Websites
{
    public class WebsiteSeed : ISeed, ISeedGenerator<SeedBuilder.LitiumGraphQlModel.Website>
    {
        private readonly Website _website;
        private string _fieldTemplateId;

        private WebsiteSeed(Website website, string fieldTemplateId)
        {
            _website = website;
            _fieldTemplateId = fieldTemplateId;
        }

        public void Commit()
        {
            var service = IoC.Resolve<WebsiteService>();

            var websiteFieldTemplateSystemId = IoC.Resolve<FieldTemplateService>()
                .Get<WebsiteFieldTemplate>(_fieldTemplateId).SystemId;

            _website.FieldTemplateSystemId = websiteFieldTemplateSystemId;
            if (_website.SystemId == Guid.Empty)
            {
                _website.SystemId = Guid.NewGuid();
                service.Create(_website);
                return;
            }

            service.Update(_website);
        }

        public static WebsiteSeed Ensure(string websiteName, string websiteTemplateName)
        {

            var websiteClone = IoC.Resolve<WebsiteService>().Get(websiteName)?.MakeWritableClone();
            if (websiteClone is null)
            {
                websiteClone = new Website(Guid.Empty)
                {
                    Id = websiteName,
                    SystemId = Guid.Empty
                };

                websiteClone.Localizations["en-US"].Name = websiteName;
            }

            return new WebsiteSeed(websiteClone, websiteTemplateName);
        }

        public static WebsiteSeed Ensure(Guid systemId, string websiteTemplateName)
        {
            var websiteService = IoC.Resolve<WebsiteService>();
            var website = websiteService.Get(systemId)?.MakeWritableClone() ?? new Website(Guid.Empty);

            return new WebsiteSeed(website, websiteTemplateName);
        }

        public static WebsiteSeed CreateFrom(SeedBuilder.LitiumGraphQlModel.Website website)
        {
            var seed = new WebsiteSeed(new Website(Guid.Empty), string.Empty);
            return (WebsiteSeed)seed.Update(website);
        }

        public ISeedGenerator<SeedBuilder.LitiumGraphQlModel.Website> Update(SeedBuilder.LitiumGraphQlModel.Website data)
        {
            _website.Id = data.Id;
            _website.FieldTemplateSystemId = Guid.Empty;
            _fieldTemplateId = data.FieldTemplate.Id;
            return this;
        }

        public void WriteMigration(StringBuilder builder)
        {
            builder.AppendLine($"\t\t\t{nameof(WebsiteSeed)}.{nameof(Ensure)}(\"{_website.Id}\",\"{_fieldTemplateId}\")");
            builder.AppendLine("\t\t\t\t.Commit();");
        }
    }
}
