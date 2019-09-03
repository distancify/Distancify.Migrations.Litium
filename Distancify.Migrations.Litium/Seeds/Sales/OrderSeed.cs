using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Litium;
using Litium.Customers;
using Litium.Foundation;
using Litium.Foundation.Modules.ECommerce;
using Litium.Foundation.Modules.ECommerce.Carriers;
using Litium.Globalization;
using Litium.Products;

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
            var order = IoC.Resolve<ModuleECommerce>().Orders.GetOrder(orderId, Solution.Instance.SystemToken)?.GetAsCarrier(true, true, true, true, true, true);
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
                    Deliveries = new List<DeliveryCarrier>(),
                    PaymentInfo = new List<PaymentInfoCarrier>()
                }, true);
            }
            return new OrderSeed(order);
        }

        public OrderSeed WithProduct(string articleNumber, decimal quantity, decimal vatPercentage = 0, decimal discountPercentage = 0)
        {
            var variant = IoC.Resolve<VariantService>().Get(articleNumber);
            var priceItem = variant.Prices.FirstOrDefault();

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
                ID = Guid.NewGuid()
            });


            return this;
        }

        public OrderSeed WithPersonCustomer(string personId)
        {
            var personService = IoC.Resolve<PersonService>();
            var person = personService.Get(personId);
            SetCustomerInfo(person, person.Addresses.First());

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
                _orderCarrier.CustomerInfo = new CustomerInfoCarrier() { ID = Guid.NewGuid() };
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
            _orderCarrier.ChannelID = channel.SystemId;
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

        public OrderSeed WithOrderStatus(short status)
        {
            _orderCarrier.OrderStatus = status;
            return this;
        }

        public OrderSeed WithPayment(Guid paymentMethodId, AddressCarrier billingAddress, string transactionReference = null)
        {
            var paymentMethod = IoC.Resolve<ModuleECommerce>().PaymentMethods.Get(paymentMethodId, Solution.Instance.SystemToken);
            var paymentInfoCarrier = _orderCarrier.PaymentInfo.FirstOrDefault();

            if (paymentInfoCarrier == null)
            {
                paymentInfoCarrier = new PaymentInfoCarrier()
                {
                    ID = Guid.NewGuid(),
                    BillingAddress = billingAddress,
                    OrderID = _orderCarrier.ID
                };
                _orderCarrier.PaymentInfo.Add(paymentInfoCarrier);
            }

            paymentInfoCarrier.PaymentMethod = paymentMethod.Name;
            paymentInfoCarrier.PaymentProvider = paymentMethod.PaymentProviderName;
            paymentInfoCarrier.ReferenceID = paymentMethod.Name;
            paymentInfoCarrier.TransactionReference = transactionReference;

            return this;
        }

        public OrderSeed WithOrderCreationDate(DateTime date)
        {
            _orderCarrier.OrderDate = date;
            return this;
        }

        public OrderSeed WithDelivery(Guid deliveryMethodId, AddressCarrier deliveryAddress, string externalReferenceId)
        {
            var delivery = IoC.Resolve<ModuleECommerce>().DeliveryMethods.Get(deliveryMethodId, Solution.Instance.SystemToken);
            var deliveryCarrier = _orderCarrier.Deliveries.FirstOrDefault();
            var cost = delivery.GetCost(_orderCarrier.CurrencyID);
            var deliveryCost = cost.IncludeVat ? cost.Cost / (1 + cost.VatPercentage / 100m) : cost.Cost;
            var deliceryCostWithVat = cost.IncludeVat ? cost.Cost : cost.Cost * (1 + cost.VatPercentage / 100m);

            if (deliveryCarrier != null)
            {
                _orderCarrier.Deliveries.Remove(deliveryCarrier);
            }

            deliveryCarrier = new DeliveryCarrier(DateTime.Now, "", deliveryCost, deliveryMethodId, 1, externalReferenceId, _orderCarrier.ID,
                DateTime.Now.AddHours(1), deliceryCostWithVat - deliveryCost, cost.VatPercentage / 100m, true,
                deliveryAddress, true, new List<AdditionalDeliveryInfoCarrier>(), Guid.Empty, 0, deliveryCost, "", true, deliceryCostWithVat);

            _orderCarrier.Deliveries.Add(deliveryCarrier);

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

        public Guid Commit()
        {
            var service = IoC.Resolve<ModuleECommerce>();
            service.Orders.CalculateOrderTotals(_orderCarrier, Solution.Instance.SystemToken);
            //service.Orders.CalculatePaymentInfoAmounts(_orderCarrier, Solution.Instance.SystemToken);

            if (_isNewOrder)
            {
                service.Orders.CreateOrder(_orderCarrier, Solution.Instance.SystemToken);
            }
            else
            {
                var order = service.Orders.GetOrder(_orderCarrier.ID, Solution.Instance.SystemToken);
                order.SetValuesFromCarrier(_orderCarrier, Solution.Instance.SystemToken);
            }

            return _orderCarrier.ID;
        }
    }
}
