using Litium;
using Litium.Common;
using Litium.Customers;
using Litium.FieldFramework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Distancify.Migrations.Litium.Seeds.Customers
{
    public class OrganizationSeed : ISeed
    {
        private readonly Organization _organization;

        private OrganizationSeed(Organization organization)
        {
            _organization = organization;
        }

        public static OrganizationSeed Ensure(string organizationId, string organizationFieldTemplateId)
        {
            var fieldTemplateSystemId = IoC.Resolve<FieldTemplateService>().Get<OrganizationFieldTemplate>(organizationFieldTemplateId).SystemId;
            var organization = IoC.Resolve<OrganizationService>().Get(organizationId)?.MakeWritableClone()
                ?? new Organization(fieldTemplateSystemId, organizationId)
                {
                    SystemId = Guid.Empty,
                    Id = organizationId
                };

            return new OrganizationSeed(organization);
        }

        public OrganizationSeed WithPersonLink(string personId)
        {
            var personSystemId = IoC.Resolve<PersonService>().Get(personId).SystemId;

            if (_organization.PersonLinks is null)
            {
                _organization.PersonLinks = new List<OrganizationToPersonLink>();
            }

            if (!_organization.PersonLinks.Any(p => p.PersonSystemId == personSystemId))
            {
                _organization.PersonLinks.Add(new OrganizationToPersonLink(personSystemId));
            }

            return this;
        }

        public OrganizationSeed WithAddress(string addressTypeId, Address address)
        {
            address.AddressTypeSystemId = IoC.Resolve<AddressTypeService>().Get(addressTypeId).SystemId;

            if (_organization.Addresses is null)
            {
                _organization.Addresses = new List<Address>();
            }

            if (!_organization.Addresses.Any(a => a.Address1.Equals(address.Address1, StringComparison.InvariantCultureIgnoreCase) &&
                                                  a.Country.Equals(address.Country, StringComparison.InvariantCultureIgnoreCase) &&
                                                  a.City.Equals(address.City, StringComparison.InvariantCultureIgnoreCase) &&
                                                  a.AddressTypeSystemId.Equals(address.AddressTypeSystemId)))
            {
                _organization.Addresses.Add(address);
            }

            return this;
        }

        public OrganizationSeed WithField(string fieldName, object value)
        {
            _organization.Fields.AddOrUpdateValue(fieldName, value);

            return this;
        }

        public Guid Commit()
        {
            var service = IoC.Resolve<OrganizationService>();

            if (_organization.SystemId.Equals(Guid.Empty))
            {
                _organization.SystemId = Guid.NewGuid();
                service.Create(_organization);
            }
            else
            {
                service.Update(_organization);
            }

            return _organization.SystemId;
        }
    }
}
