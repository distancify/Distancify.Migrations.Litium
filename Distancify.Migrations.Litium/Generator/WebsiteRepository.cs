using Distancify.Migrations.Litium.LitiumGraphqlModel;
using Distancify.Migrations.Litium.Websites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distancify.Migrations.Litium.Generator
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
