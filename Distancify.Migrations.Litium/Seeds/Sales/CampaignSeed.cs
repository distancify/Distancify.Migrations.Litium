using Litium;
using Litium.Foundation;
using Litium.Foundation.Modules.ECommerce;
using Litium.Foundation.Modules.ECommerce.Campaigns;
using Litium.Foundation.Modules.ECommerce.Carriers;
using Litium.Globalization;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Distancify.Migrations.Litium.Seeds.Sales
{
    public class CampaignSeed : ISeed
    {
        private readonly CampaignCarrier _campaignCarrier;
        private readonly bool _isNewCampaign;
        private bool _isActive = false;

        private CampaignSeed(CampaignCarrier campaignCarrier, bool isNewCampaign = false)
        {
            _campaignCarrier = campaignCarrier;
            _isNewCampaign = isNewCampaign;
        }

        public static CampaignSeed Ensure(Guid campaignId)
        {
            var campaign = IoC.Resolve<ModuleECommerce>().Campaigns.GetCampaign(campaignId, Solution.Instance.SystemToken)?.GetAsCarrier();

            if (campaign is null)
            {
                return new CampaignSeed(new CampaignCarrier() { ID = campaignId }, true);
            }

            return new CampaignSeed(campaign);
        }

        public Guid Commit()
        {
            var service = IoC.Resolve<ModuleECommerce>();

            if (_isNewCampaign)
            {
                var campaign = service.Campaigns.CreateCampaign(_campaignCarrier, Solution.Instance.SystemToken);

                if (_isActive)
                {
                    campaign.SetIsActive(true, Solution.Instance.SystemToken);
                }
            }

            return _campaignCarrier.ID;
        }

        public CampaignSeed WithName(string culture, string name)
        {
            var languageId = IoC.Resolve<LanguageService>().Get(culture).SystemId;

            if (_campaignCarrier.Names is null)
            {
                _campaignCarrier.Names = new List<CampaignNameCarrier>();
            }

            if (_campaignCarrier.Names.FirstOrDefault(n => n.LanguageID == languageId) is CampaignNameCarrier c)
            {
                c.Name = name;
            }
            else
            {
                _campaignCarrier.Names.Add(new CampaignNameCarrier
                {
                    LanguageID = languageId,
                    Name = name
                });
            }

            return this;
        }

        public CampaignSeed WithDescription(string description)
        {
            _campaignCarrier.Description = description;

            return this;
        }

        public CampaignSeed WithStartDate(DateTime startDate)
        {
            _campaignCarrier.StartDate = startDate;

            return this;
        }

        public CampaignSeed WithEndDate(DateTime endDate)
        {
            _campaignCarrier.EndDate = endDate;

            return this;
        }

        public CampaignSeed WithChannel(string channelId)
        {
            var channelSystemId = IoC.Resolve<ChannelService>().Get(channelId).SystemId;

            if (_campaignCarrier.Data.Channels is null)
            {
                _campaignCarrier.Data.Channels = new List<CampaignData.Channel>();
            }

            if (!_campaignCarrier.Data.Channels.Any(c => c.ChannelId == channelSystemId))
            {
                _campaignCarrier.Data.Channels.Add(new CampaignData.Channel() { ChannelId = channelSystemId });
            }

            return this;
        }

        public CampaignSeed WithActionInfoType(Type actionType)
        {
            _campaignCarrier.ActionInfo = new CampaignActionInfoCarrier()
            {
                TypeName = actionType.FullName
            };

            return this;
        }

        public CampaignSeed WithConditionInfo(Type conditionType, string data)
        {
            if (_campaignCarrier.ConditionInfos is null)
            {
                _campaignCarrier.ConditionInfos = new List<CampaignConditionInfoCarrier>();
            }

            var typeName = conditionType.FullName;

            if (!_campaignCarrier.ConditionInfos.Any(c => c.TypeName == typeName &&
                                                     c.Data.Equals(data, StringComparison.InvariantCultureIgnoreCase)))
            {
                _campaignCarrier.ConditionInfos.Add(new CampaignConditionInfoCarrier()
                {
                    TypeName = typeName,
                    Data = data
                });
            }

            return this;
        }

        public CampaignSeed IsActive()
        {
            _isActive = true;

            return this;
        }
    }
}
