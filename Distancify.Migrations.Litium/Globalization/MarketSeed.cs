using Litium;
using Litium.FieldFramework;
using Litium.Globalization;
using Litium.Products;
using System;
using System.Linq;

namespace Distancify.Migrations.Litium.Globalization
{
    public class MarketSeed : ISeed
    {
        private readonly Market market;

        private MarketSeed(Market market)
        {
            this.market = market;
        }

        public void Commit()
        {
            var marketService = IoC.Resolve<MarketService>();

            if (market.SystemId == Guid.Empty)
            {
                market.SystemId = Guid.NewGuid();
                marketService.Create(market);
                return;
            }

            marketService.Update(market);
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
            market.Fields.AddOrUpdateValue(id, value);
            return this;
        }

        public MarketSeed WithField(string id, string culture, object value)
        {
            market.Fields.AddOrUpdateValue(id, culture, value);
            return this;
        }

        public MarketSeed WithAssortment(string id)
        {
            market.AssortmentSystemId = IoC.Resolve<AssortmentService>().Get(id).SystemId;
            return this;
        }

        public MarketSeed WithName(string culture, string name)
        {
            if (!market.Localizations.Any(l => l.Key.Equals(culture)) ||
                !market.Localizations[culture].Name.Equals(name))
            {
                market.Localizations[culture].Name = name;
            }

            return this;
        }
    }
}