using Distancify.Migrations.Litium.Seeds.BaseSeeds;
using Litium;
using Litium.FieldFramework;
using Litium.Globalization;
using System;

namespace Distancify.Migrations.Litium.Seeds.Globalization
{
    public class MarketFieldTemplateSeed : FieldTemplateSeed<MarketFieldTemplate>
    {
        public MarketFieldTemplateSeed(MarketFieldTemplate fieldTemplate) : base(fieldTemplate)
        {
        }

        public static MarketFieldTemplateSeed Ensure(string channelFieldTemplateId)
        {
            var marketFieldTemplate = (MarketFieldTemplate)IoC.Resolve<FieldTemplateService>().Get<MarketFieldTemplate>(channelFieldTemplateId)?.MakeWritableClone();
            if (marketFieldTemplate is null)
            {
                marketFieldTemplate = new MarketFieldTemplate(channelFieldTemplateId);
                marketFieldTemplate.SystemId = Guid.Empty;
            }

            return new MarketFieldTemplateSeed(marketFieldTemplate);
        }
    }
}
