using Distancify.Migrations.Litium.Generator.Model;
using Distancify.Migrations.Litium.Globalization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distancify.Migrations.Litium.Generator.Data
{
    public class CountryRepository : Repository<Country>
    {

        public override void AppendMigration(StringBuilder builder)
        {
            foreach (var i in Items.Values)
            {
                if (i.Currency == null)
                {
                    Console.WriteLine("Error: Can't ensure currency if not currency.id is returned from GraphQL endpoint");
                    continue;
                }

                builder.AppendLine($"\t\t\t{nameof(CountrySeed)}.{nameof(CountrySeed.Ensure)}(\"{i.Id}\", \"{i.Currency.Id}\")");
                builder.AppendLine("\t\t\t\t.Commit();");
            }
        }
    }
}
