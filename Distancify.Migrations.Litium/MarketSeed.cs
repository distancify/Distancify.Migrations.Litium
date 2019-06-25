using Litium;
using Litium.FieldFramework;
using Litium.Globalization;
using Litium.Products;
using System;

namespace Distancify.Migrations.Litium
{
    public class MarketSeed : ISeed
    {
        private readonly Market Market;

        private MarketSeed(Market market)
        {
            this.Market = market;
        }

        public void Commit()
        {
            var marketService = IoC.Resolve<MarketService>();

            if (Market.SystemId == Guid.Empty)
            {
                Market.SystemId = Guid.NewGuid();
                marketService.Create(Market);
            }
            else
            {
                marketService.Update(Market);
            }
        }

        public static MarketSeed Ensure(string id, string fieldTemplateId)
        {
            var fieldTemplateSystemId = IoC.Resolve<FieldTemplateService>().Get<MarketFieldTemplate>(fieldTemplateId).SystemId;
            var market = IoC.Resolve<MarketService>().Get(id)?.MakeWritableClone() ??
                new Market(fieldTemplateSystemId)
                {
                    Id = id,
                    SystemId = Guid.Empty
                };

            return new MarketSeed(market);
        }

        public MarketSeed WithField(string id, object value)
        {
            Market.Fields.AddOrUpdateValue(id, value);
            return this;
        }

        public MarketSeed WithField(string id, string culture, object value)
        {
            Market.Fields.AddOrUpdateValue(id, culture, value);
            return this;
        }

        public MarketSeed WithAssortment(string id)
        {
            Market.AssortmentSystemId = IoC.Resolve<AssortmentService>().Get(id).SystemId;
            return this;
        }
    }
}