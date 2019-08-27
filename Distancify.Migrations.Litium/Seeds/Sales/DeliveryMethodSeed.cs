using Litium;
using Litium.Foundation;
using Litium.Foundation.Modules.ECommerce;
using Litium.Foundation.Modules.ECommerce.Carriers;
using Litium.Globalization;
using System;
using System.Collections.Generic;
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

        public DeliveryMethodSeed WithCostCarrier(string currencyId, decimal cost, bool includeVAT, decimal vatPercentage)
        {
            var currencySystemId = IoC.Resolve<CurrencyService>().Get(currencyId).SystemId;

            if (_deliveryMethodCarrier.Costs is null)
            {
                _deliveryMethodCarrier.Costs = new List<DeliveryMethodCostCarrier>();
            }

            if (_deliveryMethodCarrier.Costs.FirstOrDefault(c => c.CurrencyID == currencySystemId) is DeliveryMethodCostCarrier carrier)
            {
                carrier.Cost = cost;
                carrier.IncludeVat = includeVAT;
                carrier.VatPercentage = vatPercentage;
            }
            else
            {
                _deliveryMethodCarrier.Costs.Add(new DeliveryMethodCostCarrier()
                {
                    DeliveryMethodID = _deliveryMethodCarrier.ID,
                    CurrencyID = currencySystemId,
                    Cost = cost,
                    IncludeVat = includeVAT,
                    VatPercentage = vatPercentage
                });
            }

            return this;
        }

        public DeliveryMethodSeed WithTranslationCarrier(string languageId, string displayName)
        {
            var languageSystemId = IoC.Resolve<LanguageService>().Get(languageId).SystemId;

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
