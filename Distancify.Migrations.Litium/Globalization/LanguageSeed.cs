using Litium;
using Litium.Globalization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Graphql = Distancify.Migrations.Litium.LitiumGraphqlModel;

namespace Distancify.Migrations.Litium.Globalization
{
    public class LanguageSeed : ISeed
    {
        public const string Sweden = "SE";
        public const string UnitedKingdom = "GB";

        private readonly Language language;
        private readonly Graphql.Language graphqllanguage;

        protected LanguageSeed(Language language)
        {
            this.language = language;
        }

        public LanguageSeed(Graphql.Language graphqllanguage)
        {
            this.graphqllanguage = graphqllanguage;
        }

        public static LanguageSeed Ensure(string culture)
        {
            var languageClone = IoC.Resolve<LanguageService>().Get(culture)?.MakeWritableClone() ??
                new Language(culture)
                {
                    SystemId = Guid.Empty
                };

            return new LanguageSeed(languageClone);
        }

        public void Commit()
        {
            var service = IoC.Resolve<LanguageService>();

            if (language.SystemId == null || language.SystemId == Guid.Empty)
            {
                language.SystemId = Guid.NewGuid();
                service.Create(language);
                return;
            }

            service.Update(language);
        }

        public string GenerateMigration()
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine($"\t\t\t{nameof(LanguageSeed)}.{nameof(LanguageSeed.Ensure)}(\"{graphqllanguage.Id}\")");
            if (graphqllanguage.IsDefaultLanguage.HasValue)
            {
                builder.AppendLine($"\t\t\t\t{nameof(LanguageSeed)}.{nameof(LanguageSeed.IsDefaultLanguage)}({graphqllanguage.IsDefaultLanguage.Value.ToString().ToLower()})");
            }

            builder.AppendLine("\t\t\t\t.Commit();");
            return builder.ToString();
        }

        public LanguageSeed IsDefaultLanguage(bool isDefaultLanguage)
        {
            language.IsDefaultLanguage = isDefaultLanguage;
            return this;
        }
    }
}
