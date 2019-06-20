using Distancify.Migrations.Litium.Generator.Model;
using Distancify.Migrations.Litium.Globalization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distancify.Migrations.Litium.Generator.Data
{
    public class ChannelRepository : Repository<Channel>
    {

        public override void AppendMigration(StringBuilder builder)
        {
            foreach (var i in Items.Values)
            {
                if (i.FieldTemplate == null)
                {
                    Console.WriteLine("Error: Can't ensure channel if not fieldTemplate.id is returned from GraphQL endpoint");
                    continue;
                }

                builder.AppendLine($"\t\t\t{nameof(ChannelSeed)}.{nameof(ChannelSeed.Ensure)}(\"{i.Id}\", \"{i.FieldTemplate.Id}\")");
                foreach (var c in i.Countries)
                {
                    builder.AppendLine($"\t\t\t\t.{nameof(ChannelSeed.WithCountryLink)}(\"{c.Id}\")");
                }

                AppendFields(i, builder);

                builder.AppendLine("\t\t\t\t.Commit();");
            }
        }
    }
}
