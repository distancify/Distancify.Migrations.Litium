using Distancify.Migrations.Litium.SeedBuilder.LitiumGraphQlModel.Globalization;
using Distancify.Migrations.Litium.Seeds.Globalization;

namespace Distancify.Migrations.Litium.SeedBuilder.Repositories
{
    public class ChannelFieldTemplateRepository : Repository<ChannelFieldTemplate, ChannelFieldTemplateSeed>
    {
        protected override ChannelFieldTemplateSeed CreateFrom(ChannelFieldTemplate graphQlItem)
        {
            return ChannelFieldTemplateSeed.CreateFrom(graphQlItem);
        }
    }
}
