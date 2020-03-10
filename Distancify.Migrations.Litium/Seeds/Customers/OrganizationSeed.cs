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
        private HashSet<string> _personIds = new HashSet<string>();

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
            _personIds.Add(personId);
            
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

            if(_personIds.Count > 0)
            {
                var personService = IoC.Resolve<PersonService>();
                foreach(var personId in _personIds)
                {
                    var person = personService.Get(personId)?.MakeWritableClone();
                    if (person.OrganizationLinks.Any(l => l.OrganizationSystemId == _organization.SystemId))
                    {
                        continue;
                    }

                    person.OrganizationLinks.Add(new PersonToOrganizationLink(_organization.SystemId));
                    personService.Update(person);
                }
            }

            return _organization.SystemId;
        }
    }
}
