using Litium;
using Litium.FieldFramework;
using Litium.Globalization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distancify.Migrations.Litium.Globalization
{
    public class ChannelSeed : ISeed
    {
        public Channel channel;

        protected ChannelSeed(Channel channel)
        {
            this.channel = channel;
        }

        public static ChannelSeed Ensure(string channelName, string channelFieldTemplateId)
        {
            var templateSystemId = IoC.Resolve<FieldTemplateService>().Get<ChannelFieldTemplate>(channelFieldTemplateId).SystemId;
            var channelClone = IoC.Resolve<ChannelService>().Get(channelName)?.MakeWritableClone();
            if (channelClone is null)
            {
                channelClone = new Channel(templateSystemId);
                channelClone.SystemId = Guid.Empty;
            }

            return new ChannelSeed(channelClone);
        }

        public void Commit()
        {
            var service = IoC.Resolve<ChannelService>();

            if (channel.SystemId == null || channel.SystemId == Guid.Empty)
            {
                channel.SystemId = Guid.NewGuid();
                service.Create(channel);
                return;
            }

            service.Update(channel);
        }

        // countries
        // Market
        // Language for pages and blocks
        // Language for products
        // Websites
        // Domain
        // Domain, Url prefix
        // Setting
            // Templates
            // GTM
            // AU
            // VAT
            // Price agents
    }
}
