﻿
using Distancify.Migrations.Litium.BaseSeeds;
using Litium;
using Litium.FieldFramework;
using Litium.Customers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distancify.Migrations.Litium.Customers
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

        public void Commit()
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
    }
}
