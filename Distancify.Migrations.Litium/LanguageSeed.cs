using Litium;
using Litium.Globalization;
using System;

namespace Distancify.Migrations.Litium
{
    public class LanguageSeed : ISeed
    {
        public const string Sweden = "SE";
        public const string UnitedKingdom = "GB";

        private readonly Language Language;

        private LanguageSeed(Language language)
        {
            this.Language = language;
        }

        public void Commit()
        {
            var languageService = IoC.Resolve<LanguageService>();

            if (Language.SystemId == Guid.Empty)
            {
                Language.SystemId = Guid.NewGuid();
                languageService.Create(Language);
            }
            else
            {
                languageService.Update(Language);
            }
        }

        public static LanguageSeed Ensure(string id)
        {
            var language = IoC.Resolve<LanguageService>().Get(id)?.MakeWritableClone() ??
                new Language(id)
                {
                    SystemId = Guid.Empty
                };

            return new LanguageSeed(language);
        }

        public LanguageSeed IsDefaultLanguage(bool on)
        {
            Language.IsDefaultLanguage = on;
            return this;
        }
    }
}