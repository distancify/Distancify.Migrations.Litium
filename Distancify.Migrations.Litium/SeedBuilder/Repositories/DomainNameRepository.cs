using Distancify.Migrations.Litium.SeedBuilder.LitiumGraphQlModel.Globalization;
using Distancify.Migrations.Litium.Seeds.Globalization;

namespace Distancify.Migrations.Litium.SeedBuilder.Repositories
{
    public class DomainNameRepository : Repository<DomainName, DomainNameSeed>
    {
        protected override DomainNameSeed CreateFrom(DomainName graphQlItem)
        {
            return DomainNameSeed.CreateFrom(graphQlItem);

        }
    }
}
