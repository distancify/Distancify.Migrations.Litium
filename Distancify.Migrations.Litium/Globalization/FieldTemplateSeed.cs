using Litium;
using Litium.FieldFramework;
using Litium.Globalization;
using System;

namespace Distancify.Migrations.Litium.Globalization
{
    public class FieldTemplateSeed : ISeed
    {
        private FieldTemplate fieldTemplate;

        protected FieldTemplateSeed(FieldTemplate fieldTemplate)
        {
            this.fieldTemplate = fieldTemplate;
        }

        public static FieldTemplateSeed Ensure(string channelFieldTemplateId)
        {
            var channelFieldTemplate = IoC.Resolve<FieldTemplateService>().Get<ChannelFieldTemplate>(channelFieldTemplateId).MakeWritableClone();
            if (channelFieldTemplate is null)
            {
                channelFieldTemplate = new ChannelFieldTemplate(channelFieldTemplateId);
                channelFieldTemplate.SystemId = Guid.Empty;
            }

            return new FieldTemplateSeed(channelFieldTemplate);
        }

        public void Commit()
        {
            var service = IoC.Resolve<FieldTemplateService>();

            if (fieldTemplate.SystemId == null || fieldTemplate.SystemId == Guid.Empty)
            {
                fieldTemplate.SystemId = Guid.NewGuid();
                service.Create(fieldTemplate);
                return;
            }

            service.Update(fieldTemplate);
        }

        public FieldTemplateSeed WithName(string name, string culture)
        {
            fieldTemplate.Localizations[culture].Name = name;
            return this;
        }

        // group
        // AreaType
    }
}
