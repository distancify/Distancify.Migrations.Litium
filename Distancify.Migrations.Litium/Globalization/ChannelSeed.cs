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
                channelClone.Id = channelName;
                channelClone.SystemId = Guid.Empty;
                channelClone.Localizations["en-US"].Name = channelName;
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

        public ChannelSeed WithDomain(string name)
        {
            var domainName = IoC.Resolve<DomainNameService>().Get(name);
            if (domainName == null)
            {
                throw new NullReferenceException($"Domain {name} not found in DomainNameService");
            }

            var domainNameLink = new ChannelToDomainNameLink(domainName.SystemId);

            //TODO: Fix this
            //if (channel.DomainNameLinks.Contains(domainNameLink))
            //{
            //    return this;
            //}

            channel.DomainNameLinks.Add(domainNameLink);
            return this;
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
