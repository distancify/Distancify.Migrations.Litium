using Distancify.Migrations.Litium.SeedBuilder.LitiumGraphQlModel;
using System;
using Distancify.Migrations.Litium.Seeds.Globalization;

namespace Distancify.Migrations.Litium.SeedBuilder.Respositories
{
    public class DomainNameRepository : Repository<DomainName, DomainNameSeed>
    {
        protected override DomainNameSeed CreateFrom(DomainName graphQlItem)
        {
            return DomainNameSeed.CreateFrom(graphQlItem);

        }
    }
}
