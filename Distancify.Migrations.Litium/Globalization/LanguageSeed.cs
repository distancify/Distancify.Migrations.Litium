using Litium;
using Litium.Globalization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distancify.Migrations.Litium.Globalization
{
    public class LanguageSeed : ISeed
    {
        private Language language;

        protected LanguageSeed(Language language)
        {
            this.language = language;
        }

        public static LanguageSeed Ensure(string culture)
        {
            var languageClone = IoC.Resolve<LanguageService>().Get(culture)?.MakeWritableClone();
            if (languageClone is null)
            {
                languageClone = new Language(culture);
                languageClone.SystemId = Guid.Empty;
            }

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

        public LanguageSeed WithDefaultLanguage(bool isDefaultLanguage)
        {
            language.IsDefaultLanguage = isDefaultLanguage;
            return this;
        }
    }
}
