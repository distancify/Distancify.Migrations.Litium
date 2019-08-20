using Litium;
using Litium.FieldFramework;
using Litium.Globalization;
using Litium.Products;
using System;
using System.Linq;
using System.Text;

namespace Distancify.Migrations.Litium.Seeds.Globalization
{
    public class MarketSeed : ISeed, ISeedGenerator<SeedBuilder.LitiumGraphQlModel.Globalization.Market>
    {
        private readonly Market _market;
        private string _assortmentId;
        private string _fieldTemplateId;
        private bool _isNewMarket;

        private MarketSeed(Market market, bool isNewMarket = false)
        {
            _market = market;
            _isNewMarket = isNewMarket;
        }

        public Guid Commit()
        {
            var marketService = IoC.Resolve<MarketService>();

            if (_isNewMarket)
            {
                marketService.Create(_market);
            }
            else
            {
                marketService.Update(_market);
            }

            return _market.SystemId;
        }

        public static MarketSeed Ensure(string id, string fieldTemplateId)
        {
            var fieldTemplateSystemId = IoC.Resolve<FieldTemplateService>().Get<MarketFieldTemplate>(fieldTemplateId).SystemId;
            var market = IoC.Resolve<MarketService>().Get(id)?.MakeWritableClone();

            if (market != null)
            {
                return new MarketSeed(market);
            }

            return new MarketSeed(new Market(fieldTemplateSystemId)
            {
                Id = id,
                SystemId = Guid.NewGuid()
            }, true);
        }

        public static MarketSeed Ensure(Guid systemId, string fieldTemplateId)
        {
            var marketService = IoC.Resolve<MarketService>();
            var market = marketService.Get(systemId)?.MakeWritableClone();

            if (!(market is null))
                return new MarketSeed(market);

            var fieldTemplateService = IoC.Resolve<FieldTemplateService>();
            var fieldTemplate = fieldTemplateService.Get<MarketFieldTemplate>(fieldTemplateId);

            return new MarketSeed(new Market(fieldTemplate.SystemId) { SystemId = systemId }, true);
        }

        public MarketSeed WithField(string id, object value)
        {
            _market.Fields.AddOrUpdateValue(id, value);
            return this;
        }

        public MarketSeed WithField(string id, string culture, object value)
        {
            _market.Fields.AddOrUpdateValue(id, culture, value);
            return this;
        }

        public MarketSeed WithAssortment(string id)
        {
            _market.AssortmentSystemId = IoC.Resolve<AssortmentService>().Get(id).SystemId;
            return this;
        }

        public MarketSeed WithAssortment(Guid assortmentSystemId)
        {
            _market.AssortmentSystemId = assortmentSystemId;
            return this;
        }

        public MarketSeed WithName(string culture, string name)
        {
            if (!_market.Localizations.Any(l => l.Key.Equals(culture)) ||
                !_market.Localizations[culture].Name.Equals(name))
            {
                _market.Localizations[culture].Name = name;
            }

            return this;
        }

        public static MarketSeed CreateFrom(SeedBuilder.LitiumGraphQlModel.Globalization.Market graphQlItem)
        {
            var seed = new MarketSeed(new Market(graphQlItem.FieldTemplateSystemId));
            return (MarketSeed)seed.Update(graphQlItem);
        }

        public ISeedGenerator<SeedBuilder.LitiumGraphQlModel.Globalization.Market> Update(SeedBuilder.LitiumGraphQlModel.Globalization.Market data)
        {
            _market.SystemId = data.SystemId;
            _market.AssortmentSystemId = data.AssortmentSystemId;

            _assortmentId = data.AssortmentId;
            _fieldTemplateId = data.FieldTemplateId;

            foreach (var localization in data.Localizations)
            {
                if (!string.IsNullOrEmpty(localization.Culture) && !string.IsNullOrEmpty(localization.Name))
                {
                    _market.Localizations[localization.Culture].Name = localization.Name;
                }
                else
                {
                    this.Log().Warn("The Market with system id {MarketSystemId} contains a localization with an empty culture and/or name!", data.SystemId.ToString());
                }
            }
            return this;
        }

        public void WriteMigration(StringBuilder builder)
        {
            builder.AppendLine($"\r\n\t\t\t{nameof(MarketSeed)}.{nameof(Ensure)}(Guid.Parse(\"{_market.SystemId.ToString()}\"), \"{_fieldTemplateId}\")");

            foreach (var localization in _market.Localizations)
            {
                builder.AppendLine($"\t\t\t\t.{nameof(WithName)}(\"{localization.Key}\", \"{localization.Value.Name}\")");
            }

            if (!string.IsNullOrEmpty(_assortmentId))
            {
                builder.AppendLine($"\t\t\t\t.{nameof(WithAssortment)}(\"{_assortmentId}\")");
            }

            else if (_market.AssortmentSystemId != Guid.Empty)
            {
                builder.AppendLine($"\t\t\t\t.{nameof(WithAssortment)}(Guid.Parse({_market.AssortmentSystemId}))");
            }

            builder.AppendLine("\t\t\t\t.Commit();");
        }
    }
}