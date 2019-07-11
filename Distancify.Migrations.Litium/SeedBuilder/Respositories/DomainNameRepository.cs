using Distancify.Migrations.Litium.Seeds.Globalization;
using Distancify.Migrations.Litium.SeedBuilder.LitiumGraphQlModel.Globalization;

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
