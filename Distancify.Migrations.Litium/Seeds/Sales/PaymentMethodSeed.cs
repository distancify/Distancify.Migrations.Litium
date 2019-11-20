using Litium;
using Litium.Foundation;
using Litium.Foundation.Modules.ECommerce;
using Litium.Foundation.Modules.ECommerce.Carriers;
using Litium.Foundation.Modules.ECommerce.Payments;
using Litium.Globalization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Distancify.Migrations.Litium.Seeds.Sales
{
    public class PaymentMethodSeed : ISeed
    {
        private readonly PaymentMethodCarrier _paymentMethodCarrier;
        private readonly bool _isNewPaymentMethod;

        public PaymentMethodSeed(PaymentMethodCarrier paymentMethod, bool isNewPaymentMethod = false)
        {
            _paymentMethodCarrier = paymentMethod;
            _isNewPaymentMethod = isNewPaymentMethod;
        }

        public static PaymentMethodSeed Ensure(string paymentMethodName, string paymentProviderName)
        {
            var paymentMethod = IoC.Resolve<ModuleECommerce>().PaymentMethods.Get(paymentMethodName, paymentProviderName, Solution.Instance.SystemToken)
                                   ?.GetAsCarrier();

            if (paymentMethod is null)
            {
                return new PaymentMethodSeed(new PaymentMethodCarrier
                {
                    PaymentProviderName = paymentProviderName,
                    ID = Guid.NewGuid()
                }, true);
            }

            return new PaymentMethodSeed(paymentMethod);
        }

        public Guid Commit()
        {
            var service = IoC.Resolve<ModuleECommerce>().PaymentMethods;

            if (_isNewPaymentMethod)
            {
                var createPaymentMethod = service.GetType().GetMethod("Create", BindingFlags.NonPublic | BindingFlags.Instance);
                createPaymentMethod.Invoke(service, new object[] { _paymentMethodCarrier, Solution.Instance.SystemToken });
            }
            return _paymentMethodCarrier.ID;
        }

        public PaymentMethodSeed WithCostCarrier(string currencyId, decimal cost, bool includeVAT, decimal vatPercentage)
        {
            var currencySystemId = IoC.Resolve<CurrencyService>().Get(currencyId).SystemId;

            if (_paymentMethodCarrier.Costs is null)
            {
                _paymentMethodCarrier.Costs = new List<PaymentMethodCostCarrier>();
            }

            if (_paymentMethodCarrier.Costs.FirstOrDefault(c => c.CurrencyID == currencySystemId) is PaymentMethodCostCarrier carrier)
            {
                carrier.Cost = cost;
                carrier.IncludeVat = includeVAT;
                carrier.VatPercentage = vatPercentage;
            }
            else
            {
                _paymentMethodCarrier.Costs.Add(new PaymentMethodCostCarrier()
                {
                    PaymentMethodID = _paymentMethodCarrier.ID,
                    CurrencyID = currencySystemId,
                    Cost = cost,
                    IncludeVat = includeVAT,
                    VatPercentage = vatPercentage
                });
            }

            return this;
        }

        public PaymentMethodSeed WithTranslationCarrier(string languageId, string displayName)
        {
            var languageSystemId = IoC.Resolve<LanguageService>().Get(languageId).SystemId;

            if (_paymentMethodCarrier.Translations is null)
            {
                _paymentMethodCarrier.Translations = new List<PaymentMethodTranslationCarrier>();
            }

            if (_paymentMethodCarrier.Translations.FirstOrDefault(t => t.LanguageID == languageSystemId) is PaymentMethodTranslationCarrier carrier)
            {
                carrier.DisplayName = displayName;
            }
            else
            {
                _paymentMethodCarrier.Translations.Add(
                    new PaymentMethodTranslationCarrier(_paymentMethodCarrier.ID, languageSystemId, displayName, string.Empty));
            }

            return this;
        }

        public PaymentMethodSeed WithImage(Guid imageSystemId)
        {
            _paymentMethodCarrier.ImageID = imageSystemId;
            return this;
        }
    }
}
