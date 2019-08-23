﻿using System;
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
            var order = IoC.Resolve<ModuleECommerce>().Orders.GetOrder(orderId, Solution.Instance.SystemToken)?.GetAsCarrier();
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
                    Deliveries = new List<DeliveryCarrier>()
                }, true);
            }
            return new OrderSeed(order);
        }

        public OrderSeed WithProduct(string articleNumber, decimal quantity)
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
            var organization = organizationService.Get(person.OrganizationLinks.FirstOrDefault()?.OrganizationSystemId ?? Guid.Empty);
            SetCustomerInfo(person, organization.Addresses.First());

            return this;
        }

        private void SetCustomerInfo(Person person, Address address)
        {
            _orderCarrier.CustomerInfo = new CustomerInfoCarrier();
            _orderCarrier.CustomerInfo.CustomerNumber = person.Id;
            _orderCarrier.CustomerInfo.PersonID = person.SystemId;
            _orderCarrier.CustomerInfo.ID = Guid.NewGuid();
            _orderCarrier.CustomerInfo.Address = new AddressCarrier();
            _orderCarrier.CustomerInfo.Address.Email = person.Email;
            _orderCarrier.CustomerInfo.Address.ID = Guid.NewGuid();
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

        public OrderSeed WithPayment(Guid paymentMethodId)
        {
            var payment = IoC.Resolve<ModuleECommerce>().PaymentMethods.Get(paymentMethodId, Solution.Instance.SystemToken);
            var paymentMethod = _orderCarrier.PaymentInfo.First();
            paymentMethod.ID = payment.ID;
            paymentMethod.PaymentMethod = payment.Name;
            paymentMethod.PaymentProvider = payment.PaymentProviderName;
            paymentMethod.ReferenceID = payment.Name;

            return this;
        }

        public OrderSeed WithOrderCreationDate(DateTime date)
        {
            _orderCarrier.OrderDate = date;
            return this;
        }

        public OrderSeed WithDelivery(Guid deliveryMethodId, AddressCarrier addressCarrier)
        {
            var delivery = IoC.Resolve<ModuleECommerce>().DeliveryMethods.Get(deliveryMethodId, Solution.Instance.SystemToken);
            var deliveryCarrier = _orderCarrier.Deliveries.FirstOrDefault();

            if (deliveryCarrier == null)
            {
                deliveryCarrier = new DeliveryCarrier();
                _orderCarrier.Deliveries.Add(deliveryCarrier);
            }

            deliveryCarrier.DeliveryMethodID = deliveryMethodId;
            deliveryCarrier.DeliveryProviderID = delivery.DeliveryProviderID;
            deliveryCarrier.Address = addressCarrier;

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

            return _orderCarrier.ID;
        }
    }
}