using Distancify.Migrations.Litium.Seeds.BaseSeeds;
using Litium;
using Litium.FieldFramework;
using Litium.Globalization;
using System;

namespace Distancify.Migrations.Litium.Seeds.GlobalizationSeeds
{
    public class ChannelFieldTemplateSeed : FieldTemplateSeed<ChannelFieldTemplate>
    {
        protected ChannelFieldTemplateSeed(ChannelFieldTemplate fieldTemplate) : base(fieldTemplate)
        {
        }

        public static ChannelFieldTemplateSeed Ensure(string channelFieldTemplateId)
        {
            var channelFieldTemplate = (ChannelFieldTemplate)IoC.Resolve<FieldTemplateService>().Get<ChannelFieldTemplate>(channelFieldTemplateId)?.MakeWritableClone();
            if (channelFieldTemplate is null)
            {
                channelFieldTemplate = new ChannelFieldTemplate(channelFieldTemplateId);
                channelFieldTemplate.SystemId = Guid.Empty;
            }

            return new ChannelFieldTemplateSeed(channelFieldTemplate);
        }


        // TODO: group
        // TODO: AreaType
    }
}
