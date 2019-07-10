using Litium;
using Litium.Globalization;
using System;
using System.Text;

namespace Distancify.Migrations.Litium.Seeds.Globalization
{
    public class DomainNameSeed : ISeed, ISeedGenerator<SeedBuilder.LitiumGraphqlModel.Globalization.DomainName>
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

        internal static DomainNameSeed CreateFrom(SeedBuilder.LitiumGraphqlModel.Globalization.DomainName channel)
        {
            var seed = new DomainNameSeed(new DomainName(channel.Id));
            return (DomainNameSeed)seed.Update(channel);
        }

        public DomainNameSeed WithRobots(string robots)
        {
            domainName.Robots = robots;
            return this;
        }

        public DomainNameSeed WithHttpStrictTransportSecurityMaxAge(long? httpStrictTransportSecurityMaxAge)
        {
            domainName.HttpStrictTransportSecurityMaxAge = httpStrictTransportSecurityMaxAge;
            return this;
        }

        public ISeedGenerator<SeedBuilder.LitiumGraphqlModel.Globalization.DomainName> Update(SeedBuilder.LitiumGraphqlModel.Globalization.DomainName data)
        {
            this.domainName.Robots = data.Robots;
            this.domainName.HttpStrictTransportSecurityMaxAge = data.HttpStrictTransportSecurityMaxAge;
            return this;
        }

        public void WriteMigration(StringBuilder builder)
        {
            if (domainName == null || string.IsNullOrEmpty(domainName.Id))
            {
                throw new NullReferenceException("A DomainName with an ID obtained from the GraphQL endpoint is needed in order to ensure the DomainName");
            }


            builder.AppendLine($"\t\t\t{nameof(DomainNameSeed)}.{nameof(DomainNameSeed.Ensure)}(\"{domainName.Id}\")");
            if (!string.IsNullOrEmpty(domainName.Robots))
            {
                builder.AppendLine($"\t\t\t\t.{nameof(DomainNameSeed.WithRobots)}(\"{domainName.Robots}\")");
            }

            if (domainName.HttpStrictTransportSecurityMaxAge.HasValue)
            {
                builder.AppendLine($"\t\t\t\t.{nameof(DomainNameSeed.WithHttpStrictTransportSecurityMaxAge)}({domainName.HttpStrictTransportSecurityMaxAge.Value})");
            }

            builder.AppendLine("\t\t\t\t.Commit();");
        }
    }
}
