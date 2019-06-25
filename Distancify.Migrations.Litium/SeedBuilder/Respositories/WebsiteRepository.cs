using Distancify.Migrations.Litium.SeedBuilder.LitiumGraphqlModel;
using Distancify.Migrations.Litium.Seeds.WebsiteSeeds;
using System.Text;

namespace Distancify.Migrations.Litium.SeedBuilder.Respositories
{
    public class WebsiteRepository : Repository<Website>
    {
        public override void AppendMigration(StringBuilder builder)
        {
            foreach (var website in Items.Values)
            {
                builder.AppendLine($"\t\t\t{nameof(WebsiteSeed)}.{nameof(WebsiteSeed.Ensure)}(\"{website.Id}\",\"{website.FieldTemplate.Id}\")");
                builder.AppendLine("\t\t\t\t.Commit();");
            }
        }
    }
}
