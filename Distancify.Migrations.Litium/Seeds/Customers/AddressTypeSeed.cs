using Litium;
using Litium.Customers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distancify.Migrations.Litium.Seeds.Customers
{
    public class AddressTypeSeed : ISeed
    {
        private readonly AddressType _addressType;

        private AddressTypeSeed(AddressType addressType)
        {
            _addressType = addressType;
        }

        public static AddressTypeSeed Ensure(string addressTypeId)
        {
            var addressType = IoC.Resolve<AddressTypeService>().Get(addressTypeId)?.MakeWritableClone()
                ?? new AddressType()
                {
                    Id = addressTypeId,
                    SystemId = Guid.Empty
                };

            return new AddressTypeSeed(addressType);
        }

        public void Commit()
        {
            var service = IoC.Resolve<AddressTypeService>();

            if (_addressType.SystemId.Equals(Guid.Empty))
            {
                _addressType.SystemId = Guid.NewGuid();
                service.Create(_addressType);
            }
            else
            {
                service.Update(_addressType);
            }
        }
    }
}
