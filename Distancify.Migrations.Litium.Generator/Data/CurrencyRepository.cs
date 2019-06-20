using Distancify.Migrations.Litium.Generator.Model;
using Distancify.Migrations.Litium.Globalization;
using System.Text;

namespace Distancify.Migrations.Litium.Generator.Data
{
    public class CurrencyRepository : Repository<Currency>
    {

        public override void AppendMigration(StringBuilder builder)
        {
            foreach (var i in Items.Values)
            {
                builder.AppendLine($"\t\t\t{nameof(CurrencySeed)}.{nameof(CurrencySeed.Ensure)}(\"{i.Id}\")");
                if (i.IsBaseCurrency)
                    builder.AppendLine($"\t\t\t\t.{nameof(CurrencySeed.IsBaseCurrency)}(true)");
                builder.AppendLine("\t\t\t\t.Commit();");
            }
        }
    }
}
