using Litium;
using Litium.FieldFramework;
using Litium.Foundation;
using Litium.Foundation.Modules.ECommerce;
using Litium.Globalization;
using Litium.Websites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Distancify.Migrations.Litium.Seeds.Globalization
{
    public class ChannelSeed : ISeed, ISeedGenerator<SeedBuilder.LitiumGraphQlModel.Channel>
    {
        private readonly Channel channel;
        private string fieldTemplateId;


        private ChannelSeed(Channel channel, string fieldTemplateId)
        {
            this.fieldTemplateId = fieldTemplateId;
            this.channel = channel;
        }

        public static ChannelSeed Ensure(string identifyingField, string identifyingValue, string fieldTemplateId)
        {
            var channel = IoC.Resolve<ChannelService>().GetAll().FirstOrDefault(c => c.Fields.GetValue<string>(identifyingField).Equals(identifyingValue));
            return Ensure(channel, fieldTemplateId);
        }

        internal static ChannelSeed CreateFrom(SeedBuilder.LitiumGraphQlModel.Channel channel)
        {
            var seed = new ChannelSeed(new Channel(Guid.Empty), string.Empty);
            return (ChannelSeed)seed.Update(channel);
        }

        public static ChannelSeed Ensure(string channelId, string channelFieldTemplateId)
        {
            //var templateSystemId = IoC.Resolve<FieldTemplateService>().Get<ChannelFieldTemplate>(channelFieldTemplateId).SystemId;
            //var channelClone = IoC.Resolve<ChannelService>().Get(channelName)?.MakeWritableClone();
            //if (channelClone is null)
            //{
            //    channelClone = new Channel(templateSystemId);
            //    channelClone.Id = channelName;
            //    channelClone.SystemId = Guid.Empty;
            //    channelClone.Localizations["en-US"].Name = channelName;
            //}

            //return new ChannelSeed(channelClone);

            var channel = IoC.Resolve<ChannelService>().Get(channelId);

            if (channel is null)
            {
                channel = new Channel(Guid.Empty);
                channel.Id = channelId;
                channel.SystemId = Guid.Empty;
                channel.Localizations["en-US"].Name = channelId;
            }

            return Ensure(channel, channelFieldTemplateId);
        }

        private static ChannelSeed Ensure(Channel channel, string fieldTemplateId)
        {
            if (channel is Channel)
            {
                channel = channel.MakeWritableClone();
            }



            return new ChannelSeed(channel, fieldTemplateId);
        }

        public void Commit()
        {
            var service = IoC.Resolve<ChannelService>();

            var templateSystemId = IoC.Resolve<FieldTemplateService>().Get<ChannelFieldTemplate>(fieldTemplateId).SystemId;
            channel.FieldTemplateSystemId = templateSystemId;

            if (channel.SystemId == null || channel.SystemId == Guid.Empty)
            {
                channel.SystemId = Guid.NewGuid();
                service.Create(channel);
                return;
            }



            service.Update(channel);
        }

        public ChannelSeed WithField(string id, object value)
        {
            channel.Fields.AddOrUpdateValue(id, value);
            return this;
        }

        public ChannelSeed WithField(string id, string culture, object value)
        {
            channel.Fields.AddOrUpdateValue(id, culture, value);
            return this;
        }

        public ChannelSeed WithDomainNameLink(string domainName, bool redirect = false, string urlPrefix = null)
        {
            var domainNameSystemId = IoC.Resolve<DomainNameService>().Get(domainName).SystemId;
            ChannelToDomainNameLink domainNameLink = channel.DomainNameLinks.FirstOrDefault(link => link.DomainNameSystemId.Equals(domainNameSystemId));

            if (domainNameLink != null)
            {
                // Link exist, update the link
                domainNameLink.Redirect = redirect;
                domainNameLink.UrlPrefix = urlPrefix;
                return this;
            }

            channel.DomainNameLinks.Add(new ChannelToDomainNameLink(domainNameSystemId)
            {
                Redirect = redirect,
                UrlPrefix = urlPrefix
            });
            return this;
        }

        public ChannelSeed WithoutDomainNameLink(string domainName)
        {
            var systemId = IoC.Resolve<DomainNameService>().Get(domainName).SystemId;
            var domainNameLink = channel.DomainNameLinks.FirstOrDefault(item => item.DomainNameSystemId.Equals(systemId));
            channel.DomainNameLinks.Remove(domainNameLink);
            return this;
        }

        public ChannelSeed WithMarket(string marketId)
        {
            channel.MarketSystemId = IoC.Resolve<MarketService>().Get(marketId).SystemId;
            return this;
        }

        public ChannelSeed WithCountryLink(string id, List<string> deliveryMethodIds = null, List<string> paymentMethodIds = null)
        {
            var systemId = IoC.Resolve<CountryService>().Get(id).SystemId;
            var deliveryMethodSystemIds = deliveryMethodIds is null ? new List<Guid>() : deliveryMethodIds.Select(deliveryMethodId => ModuleECommerce.Instance.DeliveryMethods.Get(deliveryMethodId, Solution.Instance.SystemToken).ID).ToList();
            var paymentMethods = ModuleECommerce.Instance.PaymentMethods.GetAll();
            var paymentMethodSystemIds = paymentMethodIds is null ? new List<Guid>() : paymentMethodIds.Select(paymentMethodId => paymentMethods.FirstOrDefault(paymentMethod => paymentMethod.Name.Equals(paymentMethodId)).ID).ToList();
            var countryLink = channel.CountryLinks.FirstOrDefault(link => link.CountrySystemId.Equals(systemId));

            if (countryLink is ChannelToCountryLink)
            {
                foreach (var deliveryMethodSystemId in deliveryMethodSystemIds)
                {
                    if (!countryLink.DeliveryMethodSystemIds.Contains(deliveryMethodSystemId))
                    {
                        countryLink.DeliveryMethodSystemIds.Add(deliveryMethodSystemId);
                    }
                }

                foreach (var paymentMethodSystemId in paymentMethodSystemIds)
                {
                    if (!countryLink.PaymentMethodSystemIds.Contains(paymentMethodSystemId))
                    {
                        countryLink.PaymentMethodSystemIds.Add(paymentMethodSystemId);
                    }
                }
            }
            else
            {
                channel.CountryLinks.Add(new ChannelToCountryLink(systemId)
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
            var countryLink = channel.CountryLinks.FirstOrDefault(item => item.CountrySystemId.Equals(systemId));
            channel.CountryLinks.Remove(countryLink);

            return this;
        }

        public ChannelSeed WithWebsite(string id)
        {
            channel.WebsiteSystemId = string.IsNullOrEmpty(id) ? null : (Guid?)IoC.Resolve<WebsiteService>().Get(id).SystemId;
            return this;
        }

        public ChannelSeed WebsiteLanguage(string id)
        {
            channel.WebsiteLanguageSystemId = string.IsNullOrEmpty(id) ? null : (Guid?)IoC.Resolve<LanguageService>().Get(id).SystemId;
            return this;
        }

        public ChannelSeed ProductLanguage(string id)
        {
            channel.ProductLanguageSystemId = string.IsNullOrEmpty(id) ? null : (Guid?)IoC.Resolve<LanguageService>().Get(id).SystemId;
            return this;
        }

        public ChannelSeed GoogleAnalyticsAccountId(string id)
        {
            channel.GoogleAnalyticsAccountId = id;
            return this;
        }

        public ChannelSeed GoogleTagManagerContainerId(string id)
        {
            channel.GoogleTagManagerContainerId = id;
            return this;
        }

        public ChannelSeed ShowPricesWithVat(bool on)
        {
            channel.ShowPricesWithVat = on;
            return this;
        }
        public ChannelSeed PriceAgents(bool on)
        {
            channel.PriceAgents = on;
            return this;
        }

        private Dictionary<Guid, string> domainNameIdDictionary;

        public ISeedGenerator<SeedBuilder.LitiumGraphQlModel.Channel> Update(SeedBuilder.LitiumGraphQlModel.Channel channel)
        {
            if (channel.FieldTemplate == null || string.IsNullOrEmpty(channel.FieldTemplate.Id))
            {
                throw new NullReferenceException($"No fieldtemplate for channel {channel.Id}");
            }

            this.fieldTemplateId = channel.FieldTemplate.Id;
            this.channel.Id = channel.Id;
            this.channel.FieldTemplateSystemId = channel.FieldTemplate.SystemId;
            this.channel.DomainNameLinks = new List<ChannelToDomainNameLink>();
            domainNameIdDictionary = new Dictionary<Guid, string>();
            if (channel.Domains != null)
            {
                foreach (var d in channel.Domains)
                {
                    domainNameIdDictionary.Add(d.Domain.SystemId, d.Domain.Id);
                    this.channel.DomainNameLinks.Add(new ChannelToDomainNameLink(d.Domain.SystemId));
                }
            }
            return this;
        }

        public void WriteMigration(StringBuilder builder)
        {
            if (channel == null || string.IsNullOrEmpty(channel.Id))
            {
                throw new NullReferenceException("At least one Channel with an ID obtained from the GraphQL endpoint is needed in order to ensure the Channels");
            }

            if (string.IsNullOrEmpty(this.fieldTemplateId))
            {
                throw new NullReferenceException("Can't ensure channel if no ChannelFieldTemplate is returned from GraphQL endpoint");
            }


            //builder.AppendLine($"\t\t\t{nameof(ChannelSeed)}.{nameof(ChannelSeed.Ensure)}(\"{channel.Id}\", \"\")");
            builder.AppendLine($"\t\t\t{nameof(ChannelSeed)}.{nameof(ChannelSeed.Ensure)}(\"{channel.Id}\", \"{fieldTemplateId}\")");
            // WithField
            // WithField

            if (channel.DomainNameLinks != null && channel.DomainNameLinks.Count() > 0)
            {
                foreach (var d in channel.DomainNameLinks)
                {
                    if (d.DomainNameSystemId == Guid.Empty)
                    {
                        throw new NullReferenceException("Can't ensure with country link if no Domain is returned from GraphQL endpoint as part of Channel");
                    }
                    builder.Append($"\t\t\t.{nameof(ChannelSeed.WithDomainNameLink)}(\"{domainNameIdDictionary[d.DomainNameSystemId]}\"");

                    //d.Redirect //redirect
                    //d.UrlPrefix //urlPrefix
                    builder.AppendLine(")");
                }
            }

            // WithoutDomainNameLink
            // WithMarket
            //WithCountryLink
            // WithoutCountryLink
            // WithWebsite
            // ProductLanguage
            // GoogleAnalyticsAccountId
            // GoogleTagManagerContainerId
            // ShowPricesWithVat
            //PriceAgents


            //foreach (var c in channel.CountryLinks)
            //{
            //    builder.AppendLine($"\t\t\t\t.{nameof(ChannelSeed.WithCountryLink)}(\"{c.Id}\")");
            //}

            //AppendFields(i, builder);

            builder.AppendLine("\t\t\t\t.Commit();");

        }


        //TODO:  Market
        //TODO:  Language for pages and blocks
        //TODO:  Language for products
        //TODO:  Websites
        //TODO:  Domain, Url prefix
        // TODO: Setting
        //  Templates
        //  GTM
        //  AU
        //  VAT
        //  Price agents
    }
}
