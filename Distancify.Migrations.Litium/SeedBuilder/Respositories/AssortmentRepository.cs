using Distancify.Migrations.Litium.SeedBuilder.LitiumGraphqlModel;
using System.Text;
using Distancify.Migrations.Litium.Seeds.ProductSeeds;

namespace Distancify.Migrations.Litium.SeedBuilder.Respositories
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
