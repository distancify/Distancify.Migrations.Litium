using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Litium;
using Litium.FieldFramework;
using Litium.Globalization;
using Litium.Websites;

namespace Distancify.Migrations.Litium.Seeds.Websites
{
    public class WebsiteSeed : ISeed, ISeedGenerator<SeedBuilder.LitiumGraphQlModel.Websites.Website>
    {
        private readonly Website _website;
        private string _fieldTemplateId;
        private bool _isNewWebsite;

        private WebsiteSeed(Website website, string fieldTemplateId, bool isNewWebsite = false)
        {
            _website = website;
            _fieldTemplateId = fieldTemplateId;
            _isNewWebsite = isNewWebsite;
        }

        public void Commit()
        {
            var service = IoC.Resolve<WebsiteService>();

            var websiteFieldTemplateSystemId = IoC.Resolve<FieldTemplateService>()
                .Get<WebsiteFieldTemplate>(_fieldTemplateId).SystemId;

            _website.FieldTemplateSystemId = websiteFieldTemplateSystemId;

            if (_isNewWebsite)
            {
                service.Create(_website);
                return;
            }

            service.Update(_website);
        }

        public static WebsiteSeed Ensure(string websiteName, string websiteTemplateName)
        {
            var websiteClone = IoC.Resolve<WebsiteService>().Get(websiteName)?.MakeWritableClone();
            var isNewWebsite = false;

            if (websiteClone is null)
            {
                websiteClone = new Website(Guid.Empty)
                {
                    Id = websiteName,
                    SystemId = Guid.NewGuid()
                };
                isNewWebsite = true;
            }

            return new WebsiteSeed(websiteClone, websiteTemplateName, isNewWebsite);
        }

        public static WebsiteSeed Ensure(Guid systemId, string websiteTemplateName)
        {
            var websiteService = IoC.Resolve<WebsiteService>();
            var website = websiteService.Get(systemId)?.MakeWritableClone();

            var fieldTemplateService = IoC.Resolve<FieldTemplateService>();
            var websiteFieldTemplate = fieldTemplateService.Get<WebsiteFieldTemplate>(websiteTemplateName);

            if (website != null)
            {
                website.FieldTemplateSystemId = websiteFieldTemplate.SystemId;
                return new WebsiteSeed(website, websiteTemplateName);
            }

            return new WebsiteSeed(new Website(websiteFieldTemplate.SystemId) { SystemId = systemId }, websiteTemplateName, true);
        }

        public WebsiteSeed WithName(string culture, string name)
        {
            if (!_website.Localizations.Any(l => l.Key.Equals(culture)) ||
                !_website.Localizations[culture].Name.Equals(name))
            {
                _website.Localizations[culture].Name = name;
            }

            return this;
        }

        public WebsiteSeed WithField(string fieldName, Dictionary<string, object> values)
        {
            foreach (var localization in values.Keys)
            {
                _website.Fields.AddOrUpdateValue(fieldName, localization, values[localization]);
            }

            return this;
        }

        public WebsiteSeed WithField(string fieldName, object value)
        {
            _website.Fields.AddOrUpdateValue(fieldName, value);

            return this;
        }

        public static WebsiteSeed CreateFrom(SeedBuilder.LitiumGraphQlModel.Websites.Website website)
        {
            var seed = new WebsiteSeed(new Website(website.SystemId), website.FieldTemplate.Id);
            return (WebsiteSeed)seed.Update(website);
        }

        public ISeedGenerator<SeedBuilder.LitiumGraphQlModel.Websites.Website> Update(SeedBuilder.LitiumGraphQlModel.Websites.Website data)
        {
            _website.SystemId = data.SystemId;
            _fieldTemplateId = data.FieldTemplate.Id;

            foreach (var localization in data.Localizations)
            {
                if (!string.IsNullOrEmpty(localization.Culture) && !string.IsNullOrEmpty(localization.Name))
                {
                    _website.Localizations[localization.Culture].Name = localization.Name;
                }
                else
                {
                    this.Log().Warn("The website with system id {WebsiteSystemId} contains a localization with an empty culture and/or name!", data.SystemId.ToString());
                }
            }
            return this;
        }

        public void WriteMigration(StringBuilder builder)
        {
            builder.AppendLine($"\r\n\t\t\t{nameof(WebsiteSeed)}.{nameof(Ensure)}(Guid.Parse(\"{_website.SystemId.ToString()}\"), \"{_fieldTemplateId}\")");

            foreach (var localization in _website.Localizations)
            {
                builder.AppendLine($"\t\t\t\t.{nameof(WithName)}(\"{localization.Key}\", \"{localization.Value.Name}\")");
            }

            builder.AppendLine("\t\t\t\t.Commit();");
        }
    }
}
