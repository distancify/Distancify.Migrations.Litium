using Litium;
using Litium.Globalization;
using System;

namespace Distancify.Migrations.Litium.Globalization
{
    public class DomainNameSeed : ISeed
    {
        private DomainName domainName;

        protected DomainNameSeed(DomainName domainName)
        {
            this.domainName = domainName;
        }

        public static DomainNameSeed Ensure(string name)
        {
            var domainName = IoC.Resolve<DomainNameService>().Get(name)?.MakeWritableClone() ??
                new DomainName(name)
                {
                    SystemId = Guid.Empty
                };

            return new DomainNameSeed(domainName);
        }

        public void Commit()
        {
            var service = IoC.Resolve<DomainNameService>();

            if (domainName.SystemId == null || domainName.SystemId == Guid.Empty)
            {
                domainName.SystemId = Guid.NewGuid();
                service.Create(domainName);
                return;
            }

            service.Update(domainName);
        }

        public DomainNameSeed WithRobots(string robots)
        {
            domainName.Robots = robots;
            return this;
        }

        public DomainNameSeed WithHttpStrictTransportSecurityMaxAge(long? httpStrictTransportSecurityMaxAge) {
            domainName.HttpStrictTransportSecurityMaxAge = httpStrictTransportSecurityMaxAge;
            return this;
        }
    }
}
