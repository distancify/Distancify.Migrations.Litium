using Litium;
using Litium.Globalization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distancify.Migrations.Litium.Settings.Globalization
{
    public class LanguageSeed : ISeed
    {
        public const string Sweden = "SE";
        public const string UnitedKingdom = "GB";

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
