using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distancify.Migrations.Litium.Generator.Data
{
    public class Repositories
    {
        public CurrencyRepository Currencies { get; } = new CurrencyRepository();
        public CountryRepository Countries { get; } = new CountryRepository();
        public ChannelRepository Channels { get; } = new ChannelRepository();

        public string ToMigration(Config config)
        {
            var builder = new StringBuilder();
            builder.AppendLine("using Distancify.Migrations.Litium;");
            builder.AppendLine();
            builder.AppendLine($"namespace {config.Namespace}");
            builder.AppendLine("{");
            builder.AppendLine($"\tpublic class {config.ClassName} : {config.BaseMigration}");
            builder.AppendLine("\t{");
            builder.AppendLine("\t\tpublic override void Apply()");
            builder.AppendLine("\t\t{");

            Currencies.AppendMigration(builder);
            Countries.AppendMigration(builder);
            Channels.AppendMigration(builder);

            builder.AppendLine("\t\t}");
            builder.AppendLine("\t}");
            builder.AppendLine("}");

            return builder.ToString();
        }
    }
}
