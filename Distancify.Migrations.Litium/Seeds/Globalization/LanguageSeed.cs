using Litium;
using Litium.Globalization;
using System;
using System.Text;

namespace Distancify.Migrations.Litium.Seeds.Globalization
{
    public class LanguageSeed : ISeed, ISeedGenerator<SeedBuilder.LitiumGraphQlModel.Globalization.Language>
    {
        private readonly Language language;

        protected LanguageSeed(Language language)
        {
            this.language = language;
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

        public Guid Commit()
        {
            var service = IoC.Resolve<LanguageService>();

            if (language.SystemId == null || language.SystemId == Guid.Empty)
            {
                language.SystemId = Guid.NewGuid();
                service.Create(language);
            }
            else
            {
                service.Update(language);
            }

            return language.SystemId;
        }

        internal static LanguageSeed CreateFrom(SeedBuilder.LitiumGraphQlModel.Globalization.Language language)
        {
            var seed = new LanguageSeed(new Language(language.Id));
            return (LanguageSeed)seed.Update(language);
        }


        public LanguageSeed IsDefaultLanguage(bool isDefaultLanguage)
        {
            language.IsDefaultLanguage = isDefaultLanguage;
            return this;
        }

        public ISeedGenerator<SeedBuilder.LitiumGraphQlModel.Globalization.Language> Update(SeedBuilder.LitiumGraphQlModel.Globalization.Language data)
        {
            if (data.IsDefaultLanguage.HasValue)
            {
                this.language.IsDefaultLanguage = data.IsDefaultLanguage.Value;
            }
            return this;
        }

        public void WriteMigration(StringBuilder builder)
        {
            builder.AppendLine($"\r\n\t\t\t{nameof(LanguageSeed)}.{nameof(LanguageSeed.Ensure)}(\"{language.Id}\")");
            builder.AppendLine($"\t\t\t\t.{nameof(LanguageSeed.IsDefaultLanguage)}({language.IsDefaultLanguage.ToString().ToLower()})");

            builder.AppendLine("\t\t\t\t.Commit();");
        }
    }
}
