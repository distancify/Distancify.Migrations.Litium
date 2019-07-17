using System;

namespace Distancify.Migrations.Litium.SeedBuilder.LitiumGraphQlModel
{
    public class DomainName : GraphQlObject
    {
        public Guid SystemId { get; set; }
        public string Robots { get; set; }
        public int? HttpStrictTransportSecurityMaxAge { get; set; }

    }
}
