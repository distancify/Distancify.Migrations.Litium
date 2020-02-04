using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Litium;
using Litium.Customers;
using Litium.Foundation;
using Litium.Foundation.Modules.ECommerce;
using Litium.Foundation.Modules.ECommerce.Carriers;
using Litium.Foundation.Modules.ECommerce.Payments;
using Litium.Globalization;
using Litium.Products;
using Litium.Sales;
using Litium.Websites;

namespace Distancify.Migrations.Litium.Seeds.Sales
{
    public class OrderSeed : ISeed
    {
        private readonly OrderCarrier _orderCarrier;
        private readonly bool _isNewOrder;

        private OrderSeed(OrderCarrier orderCarrier, bool isNewOrder = false)
        {
            _orderCarrier = orderCarrier;
            _isNewOrder = isNewOrder;
        }

        public static OrderSeed Ensure(string orderId)
        {
            using (var md5 = MD5.Create())
            {
                var hash = md5.ComputeHash(Encoding.Default.GetBytes(orderId));
                var orderSystemId = new Guid(hash);

                return Ensure(orderSystemId);
            }
        }

        public static OrderSeed Ensure(Guid orderId)
        {
            var order = IoC.Resolve<ModuleECommerce>().Orders.GetOrder(orderId, Solution.Instance.SystemToken)?
                .GetAsCarrier(true, true, true, true, true, true);

            if (order is null)
            {
                return new OrderSeed(new OrderCarrier
                {
                    ID = orderId,
                    OrderDate = DateTime.Now,
                    Comments = "-",
                    CarrierState = new CarrierState
                    {
                        IsMarkedForCreating = true
                    },
                    Deliveries = new List<DeliveryCarrier>
                    {
                        GetDeliveryCarrier(orderId)
                    },
                    PaymentInfo = new List<PaymentInfoCarrier>(),
                    Fees = new List<FeeCarrier>(),
                    OrderDiscounts = new List<OrderDiscountCarrier>()
                }, true);
            }
            return new OrderSeed(order);
        }

        public OrderSeed WithProduct(string articleNumber, decimal quantity, decimal vatPercentage = 0, decimal discountPercentage = 0)
        {
            return AddProduct(articleNumber, quantity, vatPercentage, discountPercentage);
        }

        public OrderSeed WithProduct(string articleNumber, decimal quantity, string comments, decimal vatPercentage = 0, decimal discountPercentage = 0)
        {
            AddProduct(articleNumber, quantity, vatPercentage, discountPercentage);
            AddRowComments(articleNumber, comments);
            return this;
        }

        public OrderSeed WithProductInDeliveryState(string articleNumber, decimal quantity, short deliveryState, decimal vatPercentage = 0, decimal discountPercentage = 0)
        {
            AddProduct(articleNumber, quantity, vatPercentage, discountPercentage);
            SetRowDeliveryState(articleNumber, deliveryState);

            return this;
        }

        public OrderSeed WithProductInDeliveryState(string articleNumber, decimal quantity, string comments, short deliveryState, decimal vatPercentage = 0,
            decimal discountPercentage = 0)
        {
            AddProduct(articleNumber, quantity, vatPercentage, discountPercentage);
            SetRowDeliveryState(articleNumber, deliveryState);
            AddRowComments(articleNumber, comments);

            return this;
        }

        private OrderSeed AddProduct(string articleNumber, decimal quantity, decimal vatPercentage, decimal discountPercentage)
        {
            var variant = IoC.Resolve<VariantService>().Get(articleNumber);
            var priceListItemService = IoC.Resolve<PriceListItemService>();
            var priceItem = priceListItemService.GetByVariant(variant.SystemId).FirstOrDefault();

            if (_orderCarrier.OrderRows.FirstOrDefault(r => r.ArticleNumber.Equals(variant.Id)) is OrderRowCarrier orderRow)
            {
                orderRow.CarrierState.IsMarkedForDeleting = true;
            }

            _orderCarrier.OrderRows.Add(new OrderRowCarrier
            {
                ArticleNumber = variant.Id,
                Quantity = quantity,
                OrderID = _orderCarrier.ID,
                ExternalOrderRowID = Guid.NewGuid().ToString(),
                UnitListPrice = priceItem?.Price ?? 0,
                VATPercentage = vatPercentage,
                DiscountPercentage = discountPercentage,
                ProductID = variant.SystemId,
                ID = Guid.NewGuid(),
                DeliveryID = _orderCarrier.Deliveries.Last().ID
            });

            return this;
        }

        private void SetRowDeliveryState(string articleNumber, short deliveryState)
        {
            var orderRow = GetLastMatchingRow(articleNumber);

            var deliveryCarrier = _orderCarrier.Deliveries.FirstOrDefault(d => d.DeliveryStatus == deliveryState);
            if (deliveryCarrier == null)
            {
                deliveryCarrier = GetDeliveryCarrier(_orderCarrier.ID, deliveryState);
                _orderCarrier.Deliveries.Add(deliveryCarrier);
            }
            orderRow.DeliveryID = deliveryCarrier.ID;
        }

        private void AddRowComments(string articleNumber, string comments)
        {
            var orderRow = GetLastMatchingRow(articleNumber);
            orderRow.Comments = comments;
        }

        private OrderRowCarrier GetLastMatchingRow(string articleNumber)
        {
            var orderRow = _orderCarrier.OrderRows.Last(r => r.ArticleNumber == articleNumber);
            return orderRow;
        }

        private static DeliveryCarrier GetDeliveryCarrier(Guid orderId, short deliveryState = 0)
        {
            return new DeliveryCarrier
            {
                ID = Guid.NewGuid(),
                OrderID = orderId,
                DeliveryStatus = deliveryState,
                DeliveryMethodID = IoC.Resolve<ModuleECommerce>().DeliveryMethods.GetAll().First().ID,
                CarrierState = new CarrierState
                {
                    IsMarkedForCreating = true
                },
                Address = new AddressCarrier
                {
                    ID = Guid.NewGuid(),
                    CarrierState = new CarrierState
                    {
                        IsMarkedForCreating = true
                    },
                    Country = string.Empty,
                    Address1 = string.Empty
                }
            };
        }

        public OrderSeed WithPersonCustomer(string personId)
        {
            var personService = IoC.Resolve<PersonService>();
            var person = personService.Get(personId);
            SetCustomerInfo(person, person.Addresses.FirstOrDefault());

            return this;
        }

        public OrderSeed WithComments(string comments)
        {
            _orderCarrier.Comments = comments;
            return this;
        }

        public OrderSeed WithOrganizationCustomer(string personId)
        {
            var personService = IoC.Resolve<PersonService>();
            var organizationService = IoC.Resolve<OrganizationService>();

            var person = personService.Get(personId);
            var organizationId = person.OrganizationLinks.First().OrganizationSystemId;
            var organization = organizationService.Get(organizationId);

            SetCustomerInfo(person, organization.Addresses.First());
            _orderCarrier.CustomerInfo.OrganizationID = organizationId;

            return this;
        }

        private void SetCustomerInfo(Person person, Address address)
        {
            if (_orderCarrier.CustomerInfo == null)
            {
                _orderCarrier.CustomerInfo = new CustomerInfoCarrier { ID = Guid.NewGuid() };
            }

            _orderCarrier.CustomerInfo.CustomerNumber = person.Id;
            _orderCarrier.CustomerInfo.PersonID = person.SystemId;

            if (_orderCarrier.CustomerInfo.Address == null)
            {
                _orderCarrier.CustomerInfo.Address = new AddressCarrier { ID = Guid.NewGuid() };
            }

            _orderCarrier.CustomerInfo.Address.Email = person.Email;
            _orderCarrier.CustomerInfo.Address.FirstName = person.FirstName;
            _orderCarrier.CustomerInfo.Address.LastName = person.LastName;
            if (address != null)
            {
                _orderCarrier.CustomerInfo.Address.Phone = address.PhoneNumber;
                _orderCarrier.CustomerInfo.Address.Fax = address.PhoneNumber;
                _orderCarrier.CustomerInfo.Address.MobilePhone = address.PhoneNumber;
                _orderCarrier.CustomerInfo.Address.CareOf = address.CareOf;
                _orderCarrier.CustomerInfo.Address.Address1 = address.Address1;
                _orderCarrier.CustomerInfo.Address.Address2 = address.Address2;
                _orderCarrier.CustomerInfo.Address.City = address.City;
                _orderCarrier.CustomerInfo.Address.Zip = address.ZipCode;
                _orderCarrier.CustomerInfo.Address.Country = address.Country;
            }
        }

        public OrderSeed WithCurrency(string currencyId)
        {
            var currencyService = IoC.Resolve<CurrencyService>();
            var currency = currencyService.Get(currencyId);
            _orderCarrier.CurrencyID = currency.SystemId;

            return this;
        }

        public OrderSeed WithChannelLink(string channelId)
        {
            var channelService = IoC.Resolve<ChannelService>();
            var channel = channelService.Get(channelId);
            return WithChannelLink(channel.SystemId);
        }

        public OrderSeed WithChannelLink(Guid channelSystemId)
        {
            _orderCarrier.ChannelID = channelSystemId;
            return this;
        }


        public OrderSeed WithWebsiteLink(string websiteId)
        {
            var websiteService = IoC.Resolve<WebsiteService>();
            var website = websiteService.Get(websiteId);
            _orderCarrier.WebSiteID = website.SystemId;
            return this;
        }

        public OrderSeed WithWebsiteLink(Guid websiteSystemId)
        {
            _orderCarrier.WebSiteID = websiteSystemId;
            return this;
        }

        public OrderSeed WithCountryLink(string countryId)
        {
            var countryService = IoC.Resolve<CountryService>();
            var country = countryService.Get(countryId);
            _orderCarrier.CountryID = country.SystemId;
            return this;
        }

        public OrderSeed WithOrderType(string orderType)
        {
            _orderCarrier.AdditionalOrderInfo.Add(new AdditionalOrderInfoCarrier("Order type", _orderCarrier.ID, orderType));
            return this;
        }

        public OrderSeed WithType(OrderType type)
        {
            _orderCarrier.Type = type;
            return this;
        }

        public OrderSeed WithOrderStatus(short status)
        {
            _orderCarrier.OrderStatus = status;
            return this;
        }

        public OrderSeed WithDeliveryStatus(short status)
        {
            _orderCarrier.DeliveryStatus = status;
            _orderCarrier.Deliveries.ForEach(d => d.DeliveryStatus = status);
            return this;
        }

        public OrderSeed WithDeliveryTrackingUrl(string url)
        {
            _orderCarrier.Deliveries.ForEach(d => d.TrackingUrl = url);
            return this;
        }

        public OrderSeed WithDeliveryComment(string comment)
        {
            _orderCarrier.Deliveries.ForEach(d => d.Comments = comment);
            return this;
        }

        public OrderSeed WithPersonDeliveryAddress(string personId)
        {
            _orderCarrier.Deliveries.ForEach(d => d.Address = CreateAddressCarrier(personId));
            return this;
        }

        public class LegacyPaymentSeed : OrderSeed
        {
            private readonly PaymentInfoCarrier _payment;

            internal LegacyPaymentSeed(OrderCarrier orderCarrier, bool isNewOrder, PaymentMethod paymentMethod)
                : base(orderCarrier, isNewOrder)
            {
                _payment = new PaymentInfoCarrier()
                {
                    ID = Guid.NewGuid(),
                    OrderID = _orderCarrier.ID
                };
                _orderCarrier.PaymentInfo.Add(_payment);

                _payment.PaymentMethod = paymentMethod.Name;
                _payment.PaymentProvider = paymentMethod.PaymentProviderName;
                _payment.ReferenceID = paymentMethod.Name;
            }

            public LegacyPaymentSeed WithTransactionReference(string transactionReference)
            {
                _payment.TransactionReference = transactionReference;
                return this;
            }


            public LegacyPaymentSeed WithBillingAddress(AddressCarrier billingAddress)
            {
                _payment.BillingAddress = billingAddress;
                return this;
            }

            public LegacyPaymentSeed WithPersonBillingAddress(string personId)
            {
                _payment.BillingAddress = CreateAddressCarrier(personId);
                return this;
            }

            public LegacyPaymentSeed WithStatus(PaymentStatus status)
            {
                _payment.PaymentStatus = (short)status;
                return this;
            }
        }

        public class PaymentSeed
        {
            private readonly PaymentInfoCarrier _payment;
            private readonly OrderSeed orderSeed;

            internal PaymentSeed(OrderSeed orderSeed, OrderCarrier orderCarrier, PaymentMethod paymentMethod)
            {
                _payment = new PaymentInfoCarrier()
                {
                    ID = Guid.NewGuid(),
                    OrderID = orderCarrier.ID
                };
                orderCarrier.PaymentInfo.Add(_payment);

                _payment.PaymentMethod = paymentMethod.Name;
                _payment.PaymentProvider = paymentMethod.PaymentProviderName;
                _payment.ReferenceID = paymentMethod.Name;
                this.orderSeed = orderSeed;
            }

            public PaymentSeed WithTransactionReference(string transactionReference)
            {
                _payment.TransactionReference = transactionReference;
                return this;
            }


            public PaymentSeed WithBillingAddress(AddressCarrier billingAddress)
            {
                _payment.BillingAddress = billingAddress;
                return this;
            }

            public PaymentSeed WithPersonBillingAddress(string personId)
            {
                _payment.BillingAddress = orderSeed.CreateAddressCarrier(personId);
                return this;
            }

            public PaymentSeed WithStatus(PaymentStatus status)
            {
                _payment.PaymentStatus = (short)status;
                return this;
            }
        }

        [Obsolete("Use WithPayment(string method, string providerName, Action<PaymentSeed> paymentConfig) instead")]
        public LegacyPaymentSeed WithPayment(Guid paymentMethodId)
        {
            var paymentMethod = IoC.Resolve<ModuleECommerce>().PaymentMethods.Get(paymentMethodId, Solution.Instance.SystemToken);
            return new LegacyPaymentSeed(_orderCarrier, _isNewOrder, paymentMethod);
        }

        [Obsolete("Use WithPayment(string method, string providerName, Action<PaymentSeed> paymentConfig) instead")]
        public LegacyPaymentSeed WithPayment(string method, string providerName)
        {
            var paymentMethod = IoC.Resolve<ModuleECommerce>().PaymentMethods.Get(method, providerName, Solution.Instance.SystemToken);
            return new LegacyPaymentSeed(_orderCarrier, _isNewOrder, paymentMethod);
        }

        [Obsolete("Use WithPayment(string method, string providerName, Action<PaymentSeed> paymentConfig) instead")]
        public LegacyPaymentSeed WithPayment()
        {
            var paymentMethod = IoC.Resolve<ModuleECommerce>().PaymentMethods.GetAll().First();
            return new LegacyPaymentSeed(_orderCarrier, _isNewOrder, paymentMethod);
        }

        public OrderSeed WithPayment(string method, string providerName, Action<PaymentSeed> paymentConfig)
        {
            var paymentMethod = IoC.Resolve<ModuleECommerce>().PaymentMethods.Get(method, providerName, Solution.Instance.SystemToken);
            paymentConfig(new PaymentSeed(this, _orderCarrier, paymentMethod));
            return this;
        }

        public OrderSeed WithDelivery(Guid deliveryMethodId, AddressCarrier deliveryAddress, string externalReferenceId)
        {
            return AddDeliveryMethod(deliveryMethodId, deliveryAddress, externalReferenceId);
        }

        public OrderSeed WithDelivery(Guid deliveryMethodId, string personId, string externalReferenceId)
        {
            return AddDeliveryMethod(deliveryMethodId, CreateAddressCarrier(personId), externalReferenceId);
        }

        public OrderSeed WithDelivery(Guid deliveryMethodId, string personId)
        {
            return AddDeliveryMethod(deliveryMethodId, CreateAddressCarrier(personId), personId);
        }

        public OrderSeed WithDelivery(AddressCarrier deliveryAddress, string externalReferenceId)
        {
            var delivery = IoC.Resolve<ModuleECommerce>().DeliveryMethods.GetAll().First();
            return AddDeliveryMethod(delivery.ID, deliveryAddress, externalReferenceId);
        }

        public OrderSeed WithDelivery(string personId, string externalReferenceId, short deliveryState = 0)
        {
            var delivery = IoC.Resolve<ModuleECommerce>().DeliveryMethods.GetAll().First();
            return AddDeliveryMethod(delivery.ID, CreateAddressCarrier(personId), externalReferenceId, deliveryState);
        }

        public OrderSeed WithDelivery(string personId, short deliveryState = 0)
        {
            var delivery = IoC.Resolve<ModuleECommerce>().DeliveryMethods.GetAll().First();
            return AddDeliveryMethod(delivery.ID, CreateAddressCarrier(personId), personId, deliveryState: deliveryState);
        }

        public OrderSeed WithDefaultDelivery(Guid deliveryMethodId, AddressCarrier deliveryAddress, string externalReferenceId = null, short deliveryState = 0)
        {
            var delivery = IoC.Resolve<ModuleECommerce>().DeliveryMethods.Get(deliveryMethodId, Solution.Instance.SystemToken);

            var cost = delivery.GetCost(_orderCarrier.CurrencyID);
            var deliveryCost = cost.IncludeVat ? cost.Cost / (1 + cost.VatPercentage / 100m) : cost.Cost;
            var deliveryCostWithVat = cost.IncludeVat ? cost.Cost : cost.Cost * (1 + cost.VatPercentage / 100m);

            var deliveryCarrier = new DeliveryCarrier(DateTime.Now, "", deliveryCost, deliveryMethodId, 1, externalReferenceId, _orderCarrier.ID,
                DateTime.Now.AddHours(1), deliveryCostWithVat - deliveryCost, cost.VatPercentage / 100m, true,
                deliveryAddress, true, new List<AdditionalDeliveryInfoCarrier>(), Guid.Empty, 0, deliveryCost, "", true, deliveryCostWithVat)
            {
                ID = Guid.NewGuid(),
                DeliveryStatus = deliveryState
            };

            if (_orderCarrier.Deliveries.Any())
            {
                _orderCarrier.Deliveries.Clear();
            }

            _orderCarrier.Deliveries.Add(deliveryCarrier);
            _orderCarrier.OrderRows.ForEach(r=> r.DeliveryID = deliveryCarrier.ID);

            return this;
        }

        public OrderSeed WithDefaultDeliveryStatus(short deliveryStatus)
        {
            _orderCarrier.Deliveries.First().DeliveryStatus = deliveryStatus;
            return this;
        }

        //TODO: What happens if the currency isn't set up till this point?
        private OrderSeed AddDeliveryMethod(Guid deliveryMethodId, AddressCarrier deliveryAddress, string externalReferenceId, short deliveryState = 0)
        {
            var delivery = IoC.Resolve<ModuleECommerce>().DeliveryMethods.Get(deliveryMethodId, Solution.Instance.SystemToken);

            var deliveryCarrier = string.IsNullOrEmpty(externalReferenceId)
                ? _orderCarrier.Deliveries.FirstOrDefault(d => d.DeliveryMethodID == deliveryMethodId)
                : _orderCarrier.Deliveries.FirstOrDefault(d =>
                    d.DeliveryMethodID == deliveryMethodId && d.ExternalReferenceID == externalReferenceId);

            var cost = delivery.GetCost(_orderCarrier.CurrencyID);
            var deliveryCost = cost.IncludeVat ? cost.Cost / (1 + cost.VatPercentage / 100m) : cost.Cost;
            var deliveryCostWithVat = cost.IncludeVat ? cost.Cost : cost.Cost * (1 + cost.VatPercentage / 100m);

            if (deliveryCarrier != null)
            {
                _orderCarrier.Deliveries.Remove(deliveryCarrier);
            }

            deliveryCarrier = new DeliveryCarrier(DateTime.Now, "", deliveryCost, deliveryMethodId, 1, externalReferenceId, _orderCarrier.ID,
                DateTime.Now.AddHours(1), deliveryCostWithVat - deliveryCost, cost.VatPercentage / 100m, true,
                deliveryAddress, true, new List<AdditionalDeliveryInfoCarrier>(), Guid.Empty, 0, deliveryCost, "", true, deliveryCostWithVat)
            {
                ID = Guid.NewGuid(),
                DeliveryStatus = deliveryState
            };

            _orderCarrier.Deliveries.Add(deliveryCarrier);

            return this;
        }

        private AddressCarrier CreateAddressCarrier(string personId)
        {
            var personService = IoC.Resolve<PersonService>();
            var person = personService.Get(personId);
            var addressCarrier = new AddressCarrier
            {
                ID = Guid.NewGuid(), 
                CarrierState = new CarrierState
                {
                    IsMarkedForCreating = true
                }
            };

            var address = person.Addresses.First();

            addressCarrier.Email = person.Email;
            addressCarrier.FirstName = person.FirstName;
            addressCarrier.LastName = person.LastName;
            addressCarrier.Phone = address.PhoneNumber;
            addressCarrier.Fax = address.PhoneNumber;
            addressCarrier.MobilePhone = address.PhoneNumber;
            addressCarrier.CareOf = address.CareOf;
            addressCarrier.Address1 = address.Address1;
            addressCarrier.Address2 = address.Address2;
            addressCarrier.City = address.City;
            addressCarrier.Zip = address.ZipCode;
            addressCarrier.Country = address.Country;

            return addressCarrier;
        }

        public OrderSeed WithOrderCreationDate(DateTime date)
        {
            _orderCarrier.OrderDate = date;
            return this;
        }

        public OrderSeed WithAdditionalOrderInfo(string key, string value)
        {
            if (_orderCarrier.AdditionalOrderInfo == null)
            {
                _orderCarrier.AdditionalOrderInfo = new List<AdditionalOrderInfoCarrier>();
            }

            var additionalInfoCarrier = _orderCarrier.AdditionalOrderInfo.FirstOrDefault(x => x.Key == key && !x.CarrierState.IsMarkedForDeleting);

            if (additionalInfoCarrier == null)
            {
                _orderCarrier.AdditionalOrderInfo.Add(new AdditionalOrderInfoCarrier(key, _orderCarrier.ID, value));
            }
            else if (additionalInfoCarrier.Value != value)
            {
                additionalInfoCarrier.Value = value;
            }

            return this;
        }

        public OrderSeed WithAmountBasedOrderDiscount(decimal discountAmountWithVAT, decimal vatRate)
        {
            if (_orderCarrier.OrderDiscounts == null)
            {
                _orderCarrier.OrderDiscounts = new List<OrderDiscountCarrier>();
            }

            _orderCarrier.OrderDiscounts.Add(new OrderDiscountCarrier(
                discountAmount: discountAmountWithVAT * (1 - vatRate),
                discountDescription: $"Discount for {discountAmountWithVAT}",
                discountPercentage: 0,
                orderID: _orderCarrier.ID,
                vatAmount: discountAmountWithVAT - (discountAmountWithVAT * (1 - vatRate)),
                vatPercentage: vatRate,
                discountAmountWithVat: discountAmountWithVAT
            ));

            return this;
        }

        public OrderSeed WithPercentageBasedOrderDiscount(decimal percentage)
        {
            if (_orderCarrier.OrderDiscounts == null)
            {
                _orderCarrier.OrderDiscounts = new List<OrderDiscountCarrier>();
            }

            _orderCarrier.OrderDiscounts.Add(new OrderDiscountCarrier(
                discountAmount: 0,
                discountDescription: $"Discount for {percentage}%",
                discountPercentage: percentage,
                orderID: _orderCarrier.ID,
                vatAmount: 0,
                vatPercentage: 0,
                discountAmountWithVat: 0
            ));

            return this;
        }

        public OrderSeed WithExternalOrderId(string externalOrderId)
        {
            _orderCarrier.ExternalOrderID = externalOrderId;
            return this;
        }

        public OrderCarrier Commit()
        {
            var languageService = IoC.Resolve<LanguageService>();
            var language = languageService.Get(CultureInfo.CurrentUICulture);
            var currentCulture = CultureInfo.CurrentUICulture;
            if (language == null)
            {
                // Payment calculator in Litium depends on CurrentUICulture existing as a registered language in Litium
                CultureInfo.CurrentUICulture = languageService.GetAll().FirstOrDefault()?.CultureInfo;
            }

            var service = IoC.Resolve<ModuleECommerce>();
            service.Orders.CalculateOrderTotals(_orderCarrier, Solution.Instance.SystemToken);

            try
            {
                service.Orders.CalculatePaymentInfoAmounts(_orderCarrier, Solution.Instance.SystemToken);
            }
            catch (Exception ex)
            {
                this.Log().Debug("Failed to calculate payment info amounts for order {Order} due to {ex}", _orderCarrier.ExternalOrderID, ex);
            }
            
            if (_isNewOrder)
            {
                service.Orders.CreateOrder(_orderCarrier, Solution.Instance.SystemToken);
            }
            else
            {
                var order = service.Orders.GetOrder(_orderCarrier.ID, Solution.Instance.SystemToken);
                order.SetValuesFromCarrier(_orderCarrier, Solution.Instance.SystemToken);
            }

            // Set culture back to what it was before
            CultureInfo.CurrentUICulture = currentCulture;

            return _orderCarrier;
        }
    }
}
