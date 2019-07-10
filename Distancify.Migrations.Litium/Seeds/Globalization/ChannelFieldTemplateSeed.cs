using Distancify.Migrations.Litium.Seeds.BaseSeeds;
using Litium;
using Litium.FieldFramework;
using Litium.Globalization;
using System;
using System.Text;

namespace Distancify.Migrations.Litium.Seeds.Globalization
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

        public void WriteMigration(StringBuilder builder)
        {
            if (fieldTemplate == null || string.IsNullOrEmpty(fieldTemplate.Id))
            {
                throw new NullReferenceException("At least one Channel Field Template with an ID obtained from the GraphQL endpoint is needed in order to ensure the Channel Field Template");
            }

            builder.AppendLine($"\r\n\t\t\t{nameof(ChannelFieldTemplateSeed)}.{nameof(ChannelFieldTemplateSeed.Ensure)}(\"{fieldTemplate.Id}\")");
            builder.AppendLine("\t\t\t\t.Commit();");
        }


        // TODO: group
        // TODO: AreaType
    }
}
