using Distancify.Migrations.Litium.LitiumGraphqlModel;
using Distancify.Migrations.Litium.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distancify.Migrations.Litium.Generator
{
    public class AssortmentRepository : Repository<Assortment>
    {
        public override void AppendMigration(StringBuilder builder)
        {
            foreach (var assortment in Items.Values)
            {

                builder.AppendLine($"\t\t\t{nameof(AssortmentSeed)}.{nameof(AssortmentSeed.Ensure)}(\"{assortment.Id}\")");
                builder.AppendLine("\t\t\t\t.Commit();");

            }
        }
    }
}
