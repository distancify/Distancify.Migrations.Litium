using Distancify.Migrations.Litium.SeedBuilder.LitiumGraphqlModel.Globalization;
using Distancify.Migrations.Litium.Seeds.Globalization;

namespace Distancify.Migrations.Litium.SeedBuilder.Respositories
{
    public class LanguageRepository : Repository<Language, LanguageSeed>
    {
        protected override LanguageSeed CreateFrom(Language graphQlItem)
        {
            return LanguageSeed.CreateFrom(graphQlItem);

        }
    }
}
