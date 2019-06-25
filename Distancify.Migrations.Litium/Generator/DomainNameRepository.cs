using Distancify.Migrations.Litium.Globalization;
using Distancify.Migrations.Litium.LitiumGraphqlModel;
using System;
using System.Text;

namespace Distancify.Migrations.Litium.Generator
{
    public class DomainNameRepository : Repository<DomainName>
    {
        public override void AppendMigration(StringBuilder builder)
        {
            foreach (var domainName in Items.Values)
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
}
