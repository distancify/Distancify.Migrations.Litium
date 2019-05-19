using Litium;
using Litium.Globalization;
using System;

namespace Distancify.Migrations.Litium
{
    public class DomainNameSeed : ISeed
    {
        public const string Localhost = "localhost";

        private readonly DomainName DomainName;

        private DomainNameSeed(DomainName domainName)
        {
            this.DomainName = domainName;
        }

        public void Commit()
        {
            var domainNameService = IoC.Resolve<DomainNameService>();

            if (DomainName.SystemId == Guid.Empty)
            {
                DomainName.SystemId = Guid.NewGuid();
                domainNameService.Create(DomainName);
            }
            else
            {
                domainNameService.Update(DomainName);
            }
        }

        public static DomainNameSeed Ensure(string id)
        {
            var domainName = IoC.Resolve<DomainNameService>().Get(id)?.MakeWritableClone() ??
                new DomainName(id)
                {
                    SystemId = Guid.Empty
                };

            return new DomainNameSeed(domainName);
        }

        public DomainNameSeed WithRobots(string robots)
        {
            DomainName.Robots = robots;
            return this;
        }

        public DomainNameSeed WithHttpStrictTransportSecurityMaxAge(long? value)
        {
            DomainName.HttpStrictTransportSecurityMaxAge = value;
            return this;
        }
    }
}