using System;

namespace Distancify.Migrations.Litium.SeedBuilder.LitiumGraphqlModel
{
    public class DomainName : GraphQlObject
    {
        public Guid SystemId { get; set; }
        public string Robots { get; set; }
        public int? HttpStrictTransportSecurityMaxAge { get; set; }

    }
}
