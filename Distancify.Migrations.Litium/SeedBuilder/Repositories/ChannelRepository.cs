using Distancify.Migrations.Litium.SeedBuilder.LitiumGraphQlModel.Globalization;
using Distancify.Migrations.Litium.Seeds.Globalization;

namespace Distancify.Migrations.Litium.SeedBuilder.Repositories
{
    public class ChannelRepository : Repository<Channel, ChannelSeed>
    {

        protected override ChannelSeed CreateFrom(Channel graphQlItem)
        {
            return ChannelSeed.CreateFrom(graphQlItem);

        }
    }
}
