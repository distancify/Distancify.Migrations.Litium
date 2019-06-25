using Distancify.Migrations.Litium.Globalization;
using Distancify.Migrations.Litium.LitiumGraphqlModel;
using System.Text;

namespace Distancify.Migrations.Litium.Generator
{
    public class LanguageRepository : Repository<Language>
    {
        public override void AppendMigration(StringBuilder builder)
        {
            foreach (var language in Items.Values)
            {

                builder.AppendLine($"\t\t\t{nameof(LanguageSeed)}.{nameof(LanguageSeed.Ensure)}(\"{language.Id}\")");
                if (language.IsDefaultLanguage.HasValue)
                {
                    builder.AppendLine($"\t\t\t\t.{nameof(LanguageSeed.IsDefaultLanguage)}({language.IsDefaultLanguage.Value.ToString().ToLower()})");
                }

                builder.AppendLine("\t\t\t\t.Commit();");
            }
        }
    }
}
