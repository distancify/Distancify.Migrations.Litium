using Litium;
using Litium.Foundation;
using Litium.Foundation.Modules.ECommerce;
using Litium.Foundation.Modules.ECommerce.Carriers;
using Litium.Globalization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Distancify.Migrations.Litium.Seeds.Sales
{
    public class DeliveryMethodSeed : ISeed
    {
        private readonly DeliveryMethodCarrier _deliveryMethodCarrier;
        private bool _isNewDeliveryMethod;

        private DeliveryMethodSeed(DeliveryMethodCarrier deliveryMethodCarrier, bool isNewDeliverymethod = false)
        {
            _deliveryMethodCarrier = deliveryMethodCarrier;
            _isNewDeliveryMethod = isNewDeliverymethod;
        }

        public static DeliveryMethodSeed Ensure(string deliveryMethodName)
        {
            var deliveryMethod = IoC.Resolve<ModuleECommerce>().DeliveryMethods.Get(deliveryMethodName, Solution.Instance.SystemToken)?.GetAsCarrier();

            if (deliveryMethod is null)
            {
                return new DeliveryMethodSeed(new DeliveryMethodCarrier(deliveryMethodName) { ID = Guid.NewGuid() }, true);
            }

            return new DeliveryMethodSeed(deliveryMethod);
        }

        public Guid Commit()
        {
            var service = IoC.Resolve<ModuleECommerce>();

            if (_isNewDeliveryMethod)
            {
                service.DeliveryMethods.Create(_deliveryMethodCarrier, Solution.Instance.SystemToken);
            }

            return _deliveryMethodCarrier.ID;
        }

        public CostSeed WithCurrency(string currencyId)
        {
            var currencySystemId = IoC.Resolve<CurrencyService>().Get(currencyId).SystemId;
            return new CostSeed(_deliveryMethodCarrier, _isNewDeliveryMethod, currencySystemId);
        }

        /// <summary>
        /// Shorthand for .WithCurrency(...).WithCost(...).IsIncludeVat(...).WithVatPercentage(...)
        /// </summary>
        /// <param name="currencyId"></param>
        /// <param name="cost"></param>
        /// <param name="includeVat"></param>
        /// <param name="vatPercentage"></param>
        /// <returns></returns>
        public DeliveryMethodSeed WithCost(string currencyId, decimal cost, bool? includeVat = null, decimal? vatPercentage = null)
        {
            var seed = WithCurrency(currencyId)
                .WithCost(cost);
            if (includeVat != null) seed.IsIncludeVat((bool)includeVat);
            if (vatPercentage != null) seed.WithVatPercentage((decimal)vatPercentage);
            return this;
        }

        public class CostSeed : DeliveryMethodSeed
        {
            private readonly DeliveryMethodCostCarrier _costCarrier;

            internal CostSeed(DeliveryMethodCarrier deliveryMethodCarrier, bool isNew, Guid currencySystemId)
                : base(deliveryMethodCarrier, isNew)
            {
                if (_deliveryMethodCarrier.Costs is null)
                {
                    _deliveryMethodCarrier.Costs = new List<DeliveryMethodCostCarrier>();
                }

                _costCarrier = _deliveryMethodCarrier.Costs.FirstOrDefault(c => c.CurrencyID == currencySystemId);

                if (_costCarrier == null)
                {
                    _costCarrier = new DeliveryMethodCostCarrier();
                    _costCarrier.DeliveryMethodID = _deliveryMethodCarrier.ID;
                    _costCarrier.CurrencyID = currencySystemId;
                    _deliveryMethodCarrier.Costs.Add(_costCarrier);
                }
            }

            public CostSeed WithCost(decimal cost)
            {
                _costCarrier.Cost = cost;
                return this;
            }

            public CostSeed IsIncludeVat(bool value)
            {
                _costCarrier.IncludeVat = value;
                return this;
            }

            public CostSeed WithVatPercentage(decimal vatPercentage)
            {
                _costCarrier.VatPercentage = vatPercentage;
                return this;
            }
        }

        public DeliveryMethodSeed WithName(string languageId, string displayName)
        {
            return this.WithName(IoC.Resolve<LanguageService>().Get(languageId).SystemId, displayName);
        }

        public DeliveryMethodSeed WithName(CultureInfo language, string displayName)
        {
            return this.WithName(IoC.Resolve<LanguageService>().Get(language).SystemId, displayName);
        }

        public DeliveryMethodSeed WithName(Guid languageSystemId, string displayName)
        {
            if (_deliveryMethodCarrier.Translations is null)
            {
                _deliveryMethodCarrier.Translations = new List<DeliveryMethodTranslationCarrier>();
            }

            if (_deliveryMethodCarrier.Translations.FirstOrDefault(t => t.LanguageID == languageSystemId) is DeliveryMethodTranslationCarrier carrier)
            {
                carrier.DisplayName = displayName;
            }
            else
            {
                _deliveryMethodCarrier.Translations.Add(
                    new DeliveryMethodTranslationCarrier(_deliveryMethodCarrier.ID, languageSystemId, displayName, string.Empty));
            }

            return this;
        }

    }
}
