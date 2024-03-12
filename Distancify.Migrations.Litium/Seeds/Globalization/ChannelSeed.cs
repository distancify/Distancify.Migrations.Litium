using Litium;
using Litium.FieldFramework;
using Litium.Globalization;
using Litium.Websites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distancify.Migrations.Litium.SeedBuilder.LitiumGraphQlModel.Globalization;
using Channel = Litium.Globalization.Channel;
using ChannelFieldTemplate = Litium.Globalization.ChannelFieldTemplate;
using FieldData = Distancify.Migrations.Litium.SeedBuilder.LitiumGraphQlModel.FieldData;


namespace Distancify.Migrations.Litium.Seeds.Globalization
{
    public class ChannelSeed : ISeed, ISeedGenerator<SeedBuilder.LitiumGraphQlModel.Globalization.Channel>
    {
        private readonly Channel _channel;
        private string _fieldTemplateId;

        private string _productLanguageId;
        private string _websiteLanguageId;

        private List<FieldData> _fields;
        private bool _isNewChannel;

        private ChannelSeed(Channel channel, string fieldTemplateId, bool isNewChannel)
        {
            _fieldTemplateId = fieldTemplateId;
            _channel = channel;
            _fields = new List<FieldData>();
            _isNewChannel = isNewChannel;
        }

        public static ChannelSeed Ensure(string identifyingField, string identifyingValue, string fieldTemplateId)
        {
            var channel = IoC.Resolve<ChannelService>().GetAll().FirstOrDefault(c => c.Fields.GetValue<string>(identifyingField).Equals(identifyingValue));
            return Ensure(channel, fieldTemplateId, false);
        }

        public static ChannelSeed Ensure(string channelId, string channelFieldTemplateId)
        {
            var channel = IoC.Resolve<ChannelService>().Get(channelId)?.MakeWritableClone();
            var isNewChannel = false;

            if (channel is null)
            {
                channel = new Channel(Guid.Empty)
                {
                    Id = channelId,
                    SystemId = Guid.NewGuid()
                };
                isNewChannel = true;
            }

            return Ensure(channel, channelFieldTemplateId, isNewChannel);
        }

        public static ChannelSeed Ensure(Guid channelSystemId, string channelFieldTemplateId)
        {
            var channel = IoC.Resolve<ChannelService>().Get(channelSystemId)?.MakeWritableClone();
            var isNewChannel = false;

            if (channel is null)
            {
                channel = new Channel(Guid.Empty)
                {
                    SystemId = channelSystemId
                };
                isNewChannel = true;
            }

            return Ensure(channel, channelFieldTemplateId, isNewChannel);
        }

        private static ChannelSeed Ensure(Channel channel, string fieldTemplateId, bool isNewChannel)
        {
            if (channel is Channel)
            {
                channel = channel.MakeWritableClone();
            }

            return new ChannelSeed(channel, fieldTemplateId, isNewChannel);
        }

        public Guid Commit()
        {
            var service = IoC.Resolve<ChannelService>();
            var template = IoC.Resolve<FieldTemplateService>().Get<ChannelFieldTemplate>(_fieldTemplateId);
            if (template is null)
                throw new Exception("ChannelFieldTemplate with ID '" + _fieldTemplateId + "' not found.");

            var templateSystemId = IoC.Resolve<FieldTemplateService>().Get<ChannelFieldTemplate>(_fieldTemplateId).SystemId;
            _channel.FieldTemplateSystemId = templateSystemId;

            if (_isNewChannel)
            {
                service.Create(_channel);
            }
            else
            {
                service.Update(_channel);
            }

            return _channel.SystemId;
        }

        public ChannelSeed WithId(string id)
        {
            _channel.Id = id;
            return this;
        }

        public ChannelSeed WithField(string id, object value)
        {
            _channel.Fields.AddOrUpdateValue(id, value);
            return this;
        }

        public ChannelSeed WithField(string id, string culture, object value)
        {
            _channel.Fields.AddOrUpdateValue(id, culture, value);
            return this;
        }

        public ChannelSeed WithDomainNameLink(string domainName, bool redirect = false, string urlPrefix = null)
        {
            var domainNameSystemId = IoC.Resolve<DomainNameService>().Get(domainName).SystemId;
            var domainNameLink = _channel.DomainNameLinks.FirstOrDefault(link => link.DomainNameSystemId.Equals(domainNameSystemId));

            if (domainNameLink != null)
            {
                // Link exist, update the link
                domainNameLink.Redirect = redirect;
                domainNameLink.UrlPrefix = urlPrefix;
                return this;
            }

            _channel.DomainNameLinks.Add(new ChannelToDomainNameLink(domainNameSystemId)
            {
                Redirect = redirect,
                UrlPrefix = urlPrefix
            });

            return this;
        }

        /// <summary>
        /// Clears all domain name links
        /// </summary>
        /// <returns></returns>
        public ChannelSeed WithoutDomainNameLinks()
        {
            _channel.DomainNameLinks.Clear();
            return this;
        }

        public ChannelSeed WithoutDomainNameLink(string domainName)
        {
            var systemId = IoC.Resolve<DomainNameService>().Get(domainName).SystemId;
            var domainNameLink = _channel.DomainNameLinks.FirstOrDefault(item => item.DomainNameSystemId.Equals(systemId));
            _channel.DomainNameLinks.Remove(domainNameLink);
            return this;
        }

        public ChannelSeed WithMarket(string marketId)
        {
            _channel.MarketSystemId = IoC.Resolve<MarketService>().Get(marketId)?.SystemId;
            return this;
        }

        public ChannelSeed WithMarket(Guid marketId)
        {
            _channel.MarketSystemId = marketId;
            return this;
        }

        public ChannelSeed WithCountryLink(string countryId)
        {   
            var countrySystemId = IoC.Resolve<CountryService>().Get(countryId).SystemId;

            if (_channel.CountryLinks.FirstOrDefault(link => link.CountrySystemId.Equals(countrySystemId)) is ChannelToCountryLink countryLink)
            {
                return this;
            }

            _channel.CountryLinks.Add(new ChannelToCountryLink(countrySystemId));

            return this;
        }

        public ChannelSeed WithoutCountryLink(string id)
        {
            var systemId = IoC.Resolve<CountryService>().Get(id).SystemId;
            var countryLink = _channel.CountryLinks.FirstOrDefault(item => item.CountrySystemId.Equals(systemId));
            _channel.CountryLinks.Remove(countryLink);

            return this;
        }

        public ChannelSeed WithWebsite(string id)
        {
            _channel.WebsiteSystemId = string.IsNullOrEmpty(id) ? null : (Guid?)IoC.Resolve<WebsiteService>().Get(id).SystemId;
            return this;
        }

        public ChannelSeed WithWebsite(Guid websiteSystemId)
        {
            _channel.WebsiteSystemId = websiteSystemId;
            return this;
        }

        public ChannelSeed WithWebsiteLanguage(string id)
        {
            _channel.WebsiteLanguageSystemId = string.IsNullOrEmpty(id) ? null : (Guid?)IoC.Resolve<LanguageService>().Get(id).SystemId;
            return this;
        }

        public ChannelSeed WithProductLanguage(string id)
        {
            _channel.ProductLanguageSystemId = string.IsNullOrEmpty(id) ? null : (Guid?)IoC.Resolve<LanguageService>().Get(id).SystemId;
            return this;
        }

        public ChannelSeed WithGoogleAnalyticsAccountId(string id)
        {
            _channel.GoogleAnalyticsAccountId = id;
            return this;
        }

        public ChannelSeed WithGoogleTagManagerContainerId(string id)
        {
            _channel.GoogleTagManagerContainerId = id;
            return this;
        }

        public ChannelSeed WithShowPricesWithVat(bool on)
        {
            _channel.ShowPricesWithVat = on;
            return this;
        }

        public ChannelSeed WithPriceAgents(bool on)
        {
            _channel.PriceAgents = on;
            return this;
        }

        public ChannelSeed WithName(string culture, string name)
        {
            if (!_channel.Localizations.Any(l => l.Key.Equals(culture)) ||
                !_channel.Localizations[culture].Name.Equals(name))
            {
                _channel.Localizations[culture].Name = name;
            }

            return this;
        }

        public static ChannelSeed CreateFrom(SeedBuilder.LitiumGraphQlModel.Globalization.Channel channel)
        {
            var seed = new ChannelSeed(new Channel(Guid.Empty) { SystemId = Guid.NewGuid() }, channel.FieldTemplate.Id, false);
            return (ChannelSeed)seed.Update(channel);
        }

        private Dictionary<Guid, ChannelDomainLink> _domainNameIdDictionary;
        private Dictionary<Guid, string> _countryNameIdDictionary;

        public ISeedGenerator<SeedBuilder.LitiumGraphQlModel.Globalization.Channel> Update(SeedBuilder.LitiumGraphQlModel.Globalization.Channel channel)
        {
            if (channel.FieldTemplate == null || string.IsNullOrEmpty(channel.FieldTemplate.Id))
            {
                throw new NullReferenceException($"No field template for channel {channel.Id}");
            }

            _fieldTemplateId = channel.FieldTemplate.Id;
            _productLanguageId = channel.ProductLanguage.Id;
            _websiteLanguageId = channel.WebsiteLanguage.Id;

            _channel.Id = channel.Id;
            _channel.FieldTemplateSystemId = channel.FieldTemplate.SystemId;
            _channel.DomainNameLinks = new List<ChannelToDomainNameLink>();

            foreach (var localization in channel.Localizations)
            {
                if (!string.IsNullOrEmpty(localization.Culture) && !string.IsNullOrEmpty(localization.Name))
                {
                    _channel.Localizations[localization.Culture].Name = localization.Name;
                }
                else
                {
                    Serilog.Log.Warning("The Channel with system id {ChannelSystemId} contains a localization with an empty culture and/or name!", channel.SystemId.ToString());
                }
            }

            if (channel.Countries != null)
            {
                _countryNameIdDictionary = new Dictionary<Guid, string>();

                foreach (var c in channel.Countries)
                {
                    _countryNameIdDictionary.Add(c.SystemId, c.Id);
                    _channel.CountryLinks.Add(new ChannelToCountryLink(c.SystemId));
                }
            }

            if (channel.Domains != null)
            {
                _domainNameIdDictionary = new Dictionary<Guid, ChannelDomainLink>();

                foreach (var d in channel.Domains)
                {
                    _domainNameIdDictionary.Add(d.Domain.SystemId, d);
                    _channel.DomainNameLinks.Add(new ChannelToDomainNameLink(d.Domain.SystemId));
                }
            }

            if (channel.Market != null)
            {
                _channel.MarketSystemId = channel.Market.SystemId;
            }

            if (channel.Website != null)
            {
                _channel.WebsiteSystemId = channel.Website.SystemId;
            }

            _fields = channel.Fields?.Where(f => f.Value.Value != null || f.Value.Localizations != null)
               .Select(f => f.Value.Localizations?.Select(l => new FieldData(f.Key, l.Value, l.Culture)) ?? new[] { new FieldData(f.Key, f.Value.Value) })
               .SelectMany(f => f).Where(f => f.Value != null).ToList() ?? new List<FieldData>();


            return this;
        }

        public void WriteMigration(StringBuilder builder)
        {
            if (_channel == null || string.IsNullOrEmpty(_channel.Id))
            {
                throw new NullReferenceException("At least one Channel with an ID obtained from the GraphQL endpoint is needed in order to ensure the Channels");
            }

            if (string.IsNullOrEmpty(_fieldTemplateId))
            {
                throw new NullReferenceException("Can't ensure channel if no ChannelFieldTemplate is returned from GraphQL endpoint");
            }

            builder.AppendLine($"\r\n\t\t\t{nameof(ChannelSeed)}.{nameof(Ensure)}(\"{_channel.Id}\", \"{_fieldTemplateId}\")");

            foreach (var localization in _channel.Localizations)
            {
                builder.AppendLine($"\t\t\t\t.{nameof(WithName)}(\"{localization.Key}\", \"{localization.Value.Name}\")");
            }

            foreach (var field in _fields)
            {
                field.WriteMigration(builder);
            }

            builder.AppendLine($"\t\t\t\t.{nameof(WithProductLanguage)}(\"{_productLanguageId}\")");
            builder.AppendLine($"\t\t\t\t.{nameof(WithWebsiteLanguage)}(\"{_websiteLanguageId}\")");

            // WithField
            // WithField

            if (_channel.CountryLinks.Any())
            {
                foreach (var c in _channel.CountryLinks)
                {
                    if (c.CountrySystemId == Guid.Empty)
                    {
                        throw new NullReferenceException("Can't ensure with country link if no Country is returned from GraphQL endpoint as part of Channel");
                    }

                    builder.AppendLine($"\t\t\t\t.{nameof(WithCountryLink)}(\"{_countryNameIdDictionary[c.CountrySystemId]}\")");
                }
            }

            if (_channel.DomainNameLinks.Any())
            {
                foreach (var d in _channel.DomainNameLinks)
                {
                    if (d.DomainNameSystemId == Guid.Empty)
                    {
                        throw new NullReferenceException("Can't ensure with domain link if no Domain is returned from GraphQL endpoint as part of Channel");
                    }
                    builder.Append($"\t\t\t\t.{nameof(WithDomainNameLink)}(\"{_domainNameIdDictionary[d.DomainNameSystemId].Domain.Id}\"");
                    builder.Append($", {_domainNameIdDictionary[d.DomainNameSystemId].Redirect.ToString().ToLower()}");
                    if (!string.IsNullOrEmpty(_domainNameIdDictionary[d.DomainNameSystemId].UrlPrefix))
                    {
                        builder.Append($", \"{_domainNameIdDictionary[d.DomainNameSystemId].UrlPrefix}\"");
                    }
                    builder.AppendLine(")");
                }
            }

            if (_channel.MarketSystemId != null)
            {
                builder.AppendLine($"\t\t\t\t.{nameof(WithMarket)}(Guid.Parse(\"{_channel.MarketSystemId}\"))");
            }

            if (_channel.WebsiteSystemId != null)
            {
                builder.AppendLine($"\t\t\t\t.{nameof(WithWebsite)}(Guid.Parse(\"{_channel.WebsiteSystemId}\"))");
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
