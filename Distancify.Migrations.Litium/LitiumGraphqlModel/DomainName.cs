using System;

namespace Distancify.Migrations.Litium.LitiumGraphqlModel
{
    public class DomainName
    {
        public string Id { get; set; }
        public Guid SystemId { get; set; }
        public string Robots { get; set; }
        public int? HttpStrictTransportSecurityMaxAge { get; set; }

    }
}
