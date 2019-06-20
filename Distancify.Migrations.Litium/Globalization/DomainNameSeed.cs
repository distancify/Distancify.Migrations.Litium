using Litium;
using Litium.Globalization;
using System;
using System.Text;
using Graphql = Distancify.Migrations.Litium.LitiumGraphqlModel;

namespace Distancify.Migrations.Litium.Globalization
{
    public class DomainNameSeed : ISeed
    {
        private DomainName domainName;
        private Graphql.DomainName graphqlDomainName;

        protected DomainNameSeed(DomainName domainName)
        {
            this.domainName = domainName;
        }

        public DomainNameSeed(Graphql.DomainName graphqlDomainName)
        {
            this.graphqlDomainName = graphqlDomainName;
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

        public string GenerateMigration()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine($"\t\t\t{nameof(DomainNameSeed)}.{nameof(DomainNameSeed.Ensure)}(\"{graphqlDomainName.Id}\")");
            builder.AppendLine("\t\t\t\t.Commit();");
            return builder.ToString();
        }
    }
}
