using Litium;
using Litium.Globalization;
using System;

namespace Distancify.Migrations.Litium.Globalization
{
    public class LanguageSeed : ISeed
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


        public LanguageSeed IsDefaultLanguage(bool isDefaultLanguage)
        {
            language.IsDefaultLanguage = isDefaultLanguage;
            return this;
        }
    }
}
