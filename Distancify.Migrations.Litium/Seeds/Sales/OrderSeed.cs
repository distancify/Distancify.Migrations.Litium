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
using Litium.Foundation.Modules.ECommerce.Plugins.Campaigns;
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
            var order = IoC.Resolve<ModuleECommerce>().Orders.GetOrder(orderId, Solution.Instance.SystemToken)?.GetAsCarrier(true, true, true, true , true, true);
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

            _orderCarrier.OrderRows.Add(new OrderRowCarrier
            {
                ArticleNumber = variant.Id,
                Quantity = quantity,
                OrderID = _orderCarrier.ID,
                ExternalOrderRowID = Guid.NewGuid().ToString(),
                UnitListPrice = priceItem?.Price ?? 0,
                VATPercentage = vatPercentage,
                DiscountPercentage = discountPercentage,
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
                paymentInfoCarrier = new PaymentInfoCarrier();
                _orderCarrier.PaymentInfo.Add(paymentInfoCarrier);
            }

            paymentInfoCarrier.ID = Guid.NewGuid();
            paymentInfoCarrier.OrderID = _orderCarrier.ID;
            paymentInfoCarrier.PaymentMethod = paymentMethod.Name;
            paymentInfoCarrier.PaymentProvider = paymentMethod.PaymentProviderName;
            paymentInfoCarrier.ReferenceID = paymentMethod.Name;
            paymentInfoCarrier.BillingAddress = billingAddress;
            paymentInfoCarrier.TransactionReference = transactionReference;

            return this;
        }

        public OrderSeed WithOrderCreationDate(DateTime date)
        {
            _orderCarrier.OrderDate = date;
            return this;
        }

        public OrderSeed WithDelivery(Guid deliveryMethodId, AddressCarrier deliveryAddress)
        {
            var delivery = IoC.Resolve<ModuleECommerce>().DeliveryMethods.Get(deliveryMethodId, Solution.Instance.SystemToken);
            var deliveryCarrier = _orderCarrier.Deliveries.FirstOrDefault();

            if (deliveryCarrier == null)
            {
                deliveryCarrier = new DeliveryCarrier() { ID = Guid.NewGuid() };
                _orderCarrier.Deliveries.Add(deliveryCarrier);
            }

            deliveryCarrier.DeliveryMethodID = deliveryMethodId;
            deliveryCarrier.DeliveryProviderID = delivery.DeliveryProviderID;
            deliveryCarrier.Address = deliveryAddress;
            deliveryCarrier.OrderID = _orderCarrier.ID;

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

        public OrderSeed WithOrderCampaign(Guid campaignId, decimal discountAmountWtihVAT)
        {
            if (_orderCarrier.OrderDiscounts == null)
            {
                _orderCarrier.OrderDiscounts = new List<OrderDiscountCarrier>();
            }

            var campaign = IoC.Resolve<ModuleECommerce>().Campaigns.GetCampaign(campaignId, Solution.Instance.SystemToken);

            if (!_orderCarrier.OrderDiscounts.Any(d => d.CampaignID == campaignId))
            {
                _orderCarrier.OrderDiscounts.Add(new OrderDiscountCarrier(campaign.Description, _orderCarrier.ID, discountAmountWtihVAT, campaignId));
            }

            return this;
        }

        public Guid Commit()
        {
            var service = IoC.Resolve<ModuleECommerce>();
            if (_isNewOrder)
            {
                service.Orders.CalculateOrderTotals(_orderCarrier, Solution.Instance.SystemToken);
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
