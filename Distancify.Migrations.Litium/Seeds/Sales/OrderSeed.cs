using System;
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
                    }
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
                UnitListPrice = priceItem?.Price ?? 0
            });

            return this;
        }

        public OrderSeed WithCustomer(string personId)
        {
            var customerService = IoC.Resolve<PersonService>();
            var organizationService = IoC.Resolve<OrganizationService>();

            var person = customerService.Get(personId);
            var organization = organizationService.Get(person.OrganizationLinks.FirstOrDefault()?.OrganizationSystemId ?? Guid.Empty);
            var address = person?.Addresses?.FirstOrDefault() ?? organization?.Addresses?.FirstOrDefault() ?? new Address(Guid.Empty);

            _orderCarrier.CustomerInfo = new CustomerInfoCarrier();
            _orderCarrier.CustomerInfo.PersonID = person.SystemId;
            _orderCarrier.CustomerInfo.Address = new AddressCarrier();
            _orderCarrier.CustomerInfo.Address.CarrierState = new CarrierState { IsMarkedForUpdating = true };
            _orderCarrier.CustomerInfo.Address.Email = person.Email;

            _orderCarrier.CustomerInfo.Address.ID = address.SystemId;

            _orderCarrier.CustomerInfo.Address.FirstName = person.FirstName;
            _orderCarrier.CustomerInfo.Address.LastName = person.LastName;
            _orderCarrier.CustomerInfo.Address.Phone = address?.PhoneNumber;
            _orderCarrier.CustomerInfo.Address.Fax = address?.PhoneNumber;
            _orderCarrier.CustomerInfo.Address.MobilePhone = address?.PhoneNumber;
            _orderCarrier.CustomerInfo.Address.CareOf = address?.CareOf;
            _orderCarrier.CustomerInfo.Address.Address1 = address?.Address1;
            _orderCarrier.CustomerInfo.Address.Address2 = address?.Address2;
            _orderCarrier.CustomerInfo.Address.City = address?.City;
            _orderCarrier.CustomerInfo.Address.Zip = address?.ZipCode;
            _orderCarrier.CustomerInfo.Address.Country = address?.Country;
            return this;
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

        public void Commit()
        {
            var service = IoC.Resolve<ModuleECommerce>();
            if (_isNewOrder)
            {
                service.Orders.CalculateOrderTotals(_orderCarrier, Solution.Instance.SystemToken);
                service.Orders.CreateOrder(_orderCarrier, Solution.Instance.SystemToken);
            }
        }
    }
}
