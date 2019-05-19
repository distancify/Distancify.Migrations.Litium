using Litium;
using Litium.Globalization;
using System;
using Litium.FieldFramework;
using Litium.Websites;
using System.Linq;
using System.Collections.Generic;
using Litium.Foundation;
using Litium.Foundation.Modules.ECommerce;

namespace Distancify.Migrations.Litium
{
    public class ChannelSeed : ISeed
    {
        private readonly Channel Channel;

        private ChannelSeed(Channel channel)
        {
            this.Channel = channel;
        }

        public static ChannelSeed Ensure(string id, string fieldTemplateId)
        {
            var templateSystemId = IoC.Resolve<FieldTemplateService>().Get<ChannelFieldTemplate>(fieldTemplateId).SystemId;
            var channel = IoC.Resolve<ChannelService>().Get(id)?.MakeWritableClone() ??
                new Channel(templateSystemId) {
                    Id = id,
                    SystemId = Guid.Empty
                };

            return new ChannelSeed(channel);
        }

        public ChannelSeed WithField(string id, object value)
        {
            Channel.Fields.AddOrUpdateValue(id, value);
            return this;
        }

        public ChannelSeed WithField(string id, string culture, object value)
        {
            Channel.Fields.AddOrUpdateValue(id, culture, value);
            return this;
        }

        public ChannelSeed WithDomainNameLink(string id, bool redirect = false, string urlPrefix = null)
        {
            var systemId = IoC.Resolve<DomainNameService>().Get(id).SystemId;
            var domainNameLink = Channel.DomainNameLinks.FirstOrDefault(link => link.DomainNameSystemId.Equals(systemId));

            if (domainNameLink is ChannelToDomainNameLink)
            {
                domainNameLink.Redirect = redirect;
                domainNameLink.UrlPrefix = urlPrefix;
            }
            else
            {
                Channel.DomainNameLinks.Add(new ChannelToDomainNameLink(systemId)
                {
                    Redirect = redirect,
                    UrlPrefix = urlPrefix
                });
            }

            return this;
        }

        public ChannelSeed WithoutDomainNameLink(string id)
        {
            var systemId = IoC.Resolve<DomainNameService>().Get(id).SystemId;
            var domainNameLink = Channel.DomainNameLinks.FirstOrDefault(item => item.DomainNameSystemId.Equals(systemId));
            Channel.DomainNameLinks.Remove(domainNameLink);

            return this;
        }

        public ChannelSeed WithMarketId(string id)
        {
            Channel.MarketSystemId = IoC.Resolve<MarketService>().Get(id).SystemId;
            return this;
        }

        public ChannelSeed WithCountryLink(string id, List<string> deliveryMethodIds = null, List<string> paymentMethodIds = null)
        {
            var systemId = IoC.Resolve<CountryService>().Get(id).SystemId;
            var deliveryMethodSystemIds = deliveryMethodIds is null ? new List<Guid>() : deliveryMethodIds.Select(deliveryMethodId => ModuleECommerce.Instance.DeliveryMethods.Get(deliveryMethodId, Solution.Instance.SystemToken).ID).ToList();
            var paymentMethods = ModuleECommerce.Instance.PaymentMethods.GetAll();
            var paymentMethodSystemIds = paymentMethodIds is null ? new List<Guid>() : paymentMethodIds.Select(paymentMethodId => paymentMethods.FirstOrDefault(paymentMethod => paymentMethod.Name.Equals(paymentMethodId)).ID).ToList();

            if (!Channel.CountryLinks.Any(countryLink => countryLink.CountrySystemId.Equals(systemId)))
            {
                Channel.CountryLinks.Add(new ChannelToCountryLink(systemId)
                {
                    DeliveryMethodSystemIds = deliveryMethodSystemIds,
                    PaymentMethodSystemIds = paymentMethodSystemIds
                });
            }

            return this;
        }

        public ChannelSeed WithoutCountryLink(string id)
        {
            var systemId = IoC.Resolve<CountryService>().Get(id).SystemId;
            var countryLink = Channel.CountryLinks.FirstOrDefault(item => item.CountrySystemId.Equals(systemId));
            Channel.CountryLinks.Remove(countryLink);

            return this;
        }

        public ChannelSeed WithWebsiteId(string id)
        {
            Channel.WebsiteSystemId = string.IsNullOrEmpty(id) ? null : (Guid?)IoC.Resolve<WebsiteService>().Get(id).SystemId;
            return this;
        }

        public ChannelSeed WebsiteLanguageSystemId(string id)
        {
            Channel.WebsiteLanguageSystemId = string.IsNullOrEmpty(id) ? null : (Guid?)IoC.Resolve<LanguageService>().Get(id).SystemId;
            return this;
        }
        public ChannelSeed ProductLanguageSystemId(string id)
        {
            Channel.ProductLanguageSystemId = string.IsNullOrEmpty(id) ? null : (Guid?)IoC.Resolve<LanguageService>().Get(id).SystemId;
            return this;
        }
        public ChannelSeed GoogleAnalyticsAccountId(string id)
        {
            Channel.GoogleAnalyticsAccountId = id;
            return this;
        }
        public ChannelSeed GoogleTagManagerContainerId(string id)
        {
            Channel.GoogleTagManagerContainerId = id;
            return this;
        }
        public ChannelSeed ShowPricesWithVat(bool on)
        {
            Channel.ShowPricesWithVat = on;
            return this;
        }
        public ChannelSeed PriceAgents(bool on)
        {
            Channel.PriceAgents = on;
            return this;
        }

        public void Commit()
        {
            var channelService = IoC.Resolve<ChannelService>();

            if (Channel.SystemId == Guid.Empty)
            {
                Channel.SystemId = Guid.NewGuid();
                channelService.Create(Channel);
            }
            else
            {
                channelService.Update(Channel);
            }
        }
    }
}