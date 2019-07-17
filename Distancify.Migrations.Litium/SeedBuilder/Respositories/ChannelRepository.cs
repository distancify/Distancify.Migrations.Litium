using Distancify.Migrations.Litium.SeedBuilder.LitiumGraphQlModel;
using Distancify.Migrations.Litium.Seeds.Globalization;

namespace Distancify.Migrations.Litium.SeedBuilder.Respositories
{
    public class ChannelRepository : Repository<Channel, ChannelSeed>
    {

        
        protected override ChannelSeed CreateFrom(Channel graphQlItem)
        {
            return ChannelSeed.CreateFrom(graphQlItem);

        }
    }
}
