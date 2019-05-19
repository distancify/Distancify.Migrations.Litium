using Litium;
using Litium.FieldFramework;
using Litium.Globalization;
using Litium.Runtime;
using Litium.Websites;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Distancify.Migrations.Litium
{
    public class FieldTemplateSeed : ISeed
    {
        private readonly FieldTemplate FieldTemplate;

        private FieldTemplateSeed(FieldTemplate fieldTemplate)
        {
            this.FieldTemplate = fieldTemplate;
        }

        public void Commit()
        {
            var fieldTemplateService = IoC.Resolve<FieldTemplateService>();

            if (FieldTemplate.SystemId == Guid.Empty)
            {
                FieldTemplate.SystemId = Guid.NewGuid();
                fieldTemplateService.Create(FieldTemplate);
            }
            else
            {
                fieldTemplateService.Update(FieldTemplate);
            }
        }

        public static FieldTemplateSeed Ensure<T, TArea>(string id)
            where T : FieldTemplate<TArea>
            where TArea : IArea
        {
            var fieldTemplate = IoC.Resolve<FieldTemplateService>().Get<T>(id)?.MakeWritableClone();

            if (fieldTemplate is null)
            {
                fieldTemplate = (T)Activator.CreateInstance(typeof(T), id);
                fieldTemplate.SystemId = Guid.Empty;

                switch (fieldTemplate)
                {
                    case ChannelFieldTemplate _: (fieldTemplate as ChannelFieldTemplate).FieldGroups = new List<FieldTemplateFieldGroup>(); break;
                    case MarketFieldTemplate _: (fieldTemplate as MarketFieldTemplate).FieldGroups = new List<FieldTemplateFieldGroup>(); break;
                    case WebsiteFieldTemplate _: (fieldTemplate as WebsiteFieldTemplate).FieldGroups = new List<FieldTemplateFieldGroup>(); break;
                    case PageFieldTemplate _: (fieldTemplate as PageFieldTemplate).FieldGroups = new List<FieldTemplateFieldGroup>(); break;
                }
            }

            return new FieldTemplateSeed(fieldTemplate);
        }

        public FieldTemplateSeed WithNames(Dictionary<string, string> localizedNamesByCulture)
        {
            foreach (var item in localizedNamesByCulture)
            {
                if (!FieldTemplate.Localizations.Any(l => l.Key.Equals(item.Key)) ||
                    !FieldTemplate.Localizations[item.Key].Name.Equals(item.Value))
                {
                    FieldTemplate.Localizations[item.Key].Name = item.Value;
                }
            }

            return this;
        }

        public FieldTemplateSeed WithFieldGroups(List<FieldTemplateFieldGroup> newFieldGroups)
        {
            ICollection<FieldTemplateFieldGroup> currentFieldGroups = null;

            switch (FieldTemplate)
            {
                case ChannelFieldTemplate channelFieldTemplate: currentFieldGroups = channelFieldTemplate.FieldGroups; break;
                case MarketFieldTemplate marketFieldTemplate: currentFieldGroups = marketFieldTemplate.FieldGroups; break;
                case WebsiteFieldTemplate websiteFieldTemplate: currentFieldGroups = websiteFieldTemplate.FieldGroups; break;
                case PageFieldTemplate pageFieldTemplate: currentFieldGroups = pageFieldTemplate.FieldGroups; break;
            }

            foreach (var newFieldGroup in newFieldGroups)
            {
                var currentFieldGroup = currentFieldGroups.FirstOrDefault(g => g.Id.Equals(newFieldGroup.Id));

                if (currentFieldGroup == null)
                {
                    currentFieldGroups.Add(newFieldGroup);
                }
                else
                {
                    foreach (var item in newFieldGroup.Localizations)
                    {
                        if (!currentFieldGroup.Localizations.Any(l => l.Key.Equals(item.Key)) ||
                            !currentFieldGroup.Localizations[item.Key].Name.Equals(item.Value))
                        {
                            currentFieldGroup.Localizations[item.Key].Name = item.Value.Name;
                        }
                    }

                    foreach (var field in newFieldGroup.Fields)
                    {
                        if (!currentFieldGroup.Fields.Contains(field))
                        {
                            currentFieldGroup.Fields.Add(field);
                        }
                    }
                }
            }

            return this;
        }
    }
}