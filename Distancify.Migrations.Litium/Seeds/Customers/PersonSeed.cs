using Litium;
using Litium.FieldFramework;
using Litium.Customers;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Distancify.Migrations.Litium.Seeds.Customers
{
    public class PersonSeed : ISeed
    {
        private readonly Person person;

        protected PersonSeed(Person person)
        {
            this.person = person;
        }

        public static PersonSeed EnsureSystem(string fieldTemplateId)
        {
            var templateSystemId = IoC.Resolve<FieldTemplateService>().Get<PersonFieldTemplate>(fieldTemplateId).SystemId;
            var person = IoC.Resolve<PersonService>().Get("_system")?.MakeWritableClone();

            if (person is null)
            {
                person = new Person(templateSystemId);
                person.Id = "_system";
                person.SystemId = Guid.Empty;
            }

            person.Fields[SystemFieldDefinitionConstants.FirstName] = "System";
            person.LoginCredential.Username = "_system";

            return new PersonSeed(person);
        }

        public static PersonSeed EnsureEveryone(string fieldTemplateId)
        {
            var templateSystemId = IoC.Resolve<FieldTemplateService>().Get<PersonFieldTemplate>(fieldTemplateId).SystemId;
            var person = IoC.Resolve<PersonService>().Get("_everyone")?.MakeWritableClone();

            if (person is null)
            {
                person = new Person(templateSystemId);
                person.Id = "_everyone";
                person.SystemId = Guid.Empty;
            }

            person.Fields[SystemFieldDefinitionConstants.FirstName] = "Everyone";
            person.LoginCredential.Username = "_everyone";

            return new PersonSeed(person);
        }

        public static PersonSeed Ensure(string id, string fieldTemplateId)
        {
            var templateSystemId = IoC.Resolve<FieldTemplateService>().Get<PersonFieldTemplate>(fieldTemplateId).SystemId;
            var person = IoC.Resolve<PersonService>().Get(id)?.MakeWritableClone();

            if (person is null)
            {
                person = new Person(templateSystemId);
                person.Id = id;
                person.SystemId = Guid.Empty;
            }

            return new PersonSeed(person);
        }

        public Guid Commit()
        {
            var service = IoC.Resolve<PersonService>();

            if (person.SystemId == Guid.Empty)
            {

                if (person.Id == "_system")
                {
                    person.SystemId = Guid.Parse("00000000-0000-0000-0000-000000000001");
                }
                else if (person.Id == "_everyone")
                {
                    person.SystemId = Guid.Parse("00000000-0000-0000-0000-000000000002");
                }
                else
                {
                    person.SystemId = Guid.NewGuid();
                }

                service.Create(person);
            }
            else
            {
                service.Update(person);
            }

            return person.SystemId;
        }

        public PersonSeed WithFirstName(string firstName)
        {
            person.Fields[SystemFieldDefinitionConstants.FirstName] = firstName;
            return this;
        }

        public PersonSeed WithLastName(string lastName)
        {
            person.Fields[SystemFieldDefinitionConstants.LastName] = lastName;
            return this;
        }

        public PersonSeed WithGroupLink(string groupId)
        {
            var group = IoC.Resolve<GroupService>().Get<Group>(groupId);

            if (group != null && !person.GroupLinks.Any(r => r.GroupSystemId == group.SystemId))
            {
                person.GroupLinks.Add(new PersonToGroupLink(group.SystemId));
            }

            return this;
        }

        public PersonSeed WithLogin(string username, string password)
        {
            person.LoginCredential.Username = username;
            person.LoginCredential.NewPassword = password;
            return this;
        }

        public PersonSeed WithEmail(string email)
        {
            person.Email = email;

            return this;
        }

        public PersonSeed WithAddress(Address address)
        {
            if (person.Addresses == null)
            {
                person.Addresses = new List<Address>();
            }

            var existing = person.Addresses.FirstOrDefault(r => r.SystemId == address.SystemId);
            if (existing == null)
            {
                person.Addresses.Add(address);
            }
            else
            {
                existing.Address1 = address.Address1;
                existing.Address2 = address.Address2;
                existing.AddressTypeSystemId = address.AddressTypeSystemId;
                existing.CareOf = address.CareOf;
                existing.City = address.City;
                existing.Country = address.Country;
                existing.CustomData = address.CustomData;
                existing.HouseExtension = address.HouseExtension;
                existing.HouseNumber = address.HouseNumber;
                existing.PhoneNumber = address.PhoneNumber;
                existing.State = address.State;
                existing.ZipCode = address.ZipCode;
            }

            return this;
        }
    }
}
