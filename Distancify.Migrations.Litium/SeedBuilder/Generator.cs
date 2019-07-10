using Distancify.Migrations.Litium.SeedBuilder.LitiumGraphqlModel;
using Distancify.Migrations.Litium.SeedBuilder.Respositories;
using Distancify.Migrations.Litium.Seeds;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Distancify.Migrations.Litium.SeedBuilder
{
    public class Generator : IGenerator
    {
        //public List<ISeed> seeds;
        private ChannelRepository channelSeedRespository = new ChannelRepository();
        private CountryRepository countrySeedRespository = new CountryRepository();
        private DomainNameRepository domainNameSeedRespository = new DomainNameRepository();
        private CurrencyRepository currencySeedRespository = new CurrencyRepository();
        private LanguageRepository languageSeedRespository = new LanguageRepository();
        private WebsiteRepository websiteSeedRespository = new WebsiteRepository();
        private AssortmentRepository assortmentSeedRespository = new AssortmentRepository();

        public int NumberOfSeeds
        {
            get {
                int seedsCount = 0;
                seedsCount += channelSeedRespository.NumberOfItems;
                seedsCount += countrySeedRespository.NumberOfItems;
                seedsCount += domainNameSeedRespository.NumberOfItems;
                seedsCount += currencySeedRespository.NumberOfItems;
                seedsCount += languageSeedRespository.NumberOfItems;
                seedsCount += websiteSeedRespository.NumberOfItems;
                seedsCount += assortmentSeedRespository.NumberOfItems;
                return seedsCount;
            }
        }

        public string GenerateMigration()
        {
            //TODO: PersonFieldTemplateSeed (graphql)
            //TODO: PersonSeed (graphql)
            //DomainNameSeed, robots + maxage
            //CurrencySeed, isBaseCurrency
            //TODO: TaxClassSeed (graphql)
            //?? Culture
            //?? FieldDefinition
            //?? Field
            //?? FieldTemplateFieldGroup
            //?? LitiumEntity
            //CountrySeed + standardrate
            //LanguageSeed + isdefaultlanguage
            //TODO: MarketFieldTemplateSeed (graphql)
            //TODO: MarketSeed
            //?? ChannelFields
            //?? UrlDirect
            //TODO: ChannelFieldTemplateSeed
            //AssortmentSeed
            //TODO: PageFieldTemplateSeed (graphql)
            //?? WebsiteFields
            //TODO: WebsiteFieldTemplateSeed
            //WebsiteSeed
            //?? WebsiteText
            // ChannelSeed + domainNameLink
            //TODO: ProductDisplayTemplateSeed (graphql)
            //TODO: ProductFieldTemplateSeed (graphql)
            //TODO: CategoryDisplayTemplateSeed (graphql)
            //TODO: CategoryFieldTemplateSeed (graphql)
            //TODO: AssortmentCategorySeed (graphql)
            //TODO: BaseProductSeed (graphql)
            //TODO: PriceListSeed (graphql)
            //TODO: InventorySeed (graphql)
            //TODO: UnitOfMeasurementSeed (graphql)
            //TODO: VariantSeed (graphql)
            //TODO: BlockCategorySeed (graphql)
            //TODO: BlockFieldTemplateSeed (graphql)
            //TODO: BlockSeed (graphql)
            //TODO: PageSeed (graphql)

            StringBuilder migrationBuilder = new StringBuilder();
            domainNameSeedRespository.WriteMigration(migrationBuilder);
            currencySeedRespository.WriteMigration(migrationBuilder);
            countrySeedRespository.WriteMigration(migrationBuilder);
            languageSeedRespository.WriteMigration(migrationBuilder);
            assortmentSeedRespository.WriteMigration(migrationBuilder);
            websiteSeedRespository.WriteMigration(migrationBuilder);
            channelSeedRespository.WriteMigration(migrationBuilder);


            return migrationBuilder.ToString();
        }

       
        public void PopulateSeedsWithData(LitiumGraphqlModel.Data data)
        {
            //if (data.Globalization.DomainNames != null)
            //{
            //    foreach (var d in data.Globalization.DomainNames)
            //    {
            //        domainNameSeedRespository.AddOrMerge(d);
            //    }
            //}

            AddOrMerge(domainNameSeedRespository, data.Globalization.DomainNames);

            //if (data.Globalization.Currencies != null)
            //{
            //    foreach (var c in data.Globalization.Currencies)
            //    {
            //        currencySeedRespository.AddOrMerge(c);
            //    }
            //}

            AddOrMerge(currencySeedRespository, data.Globalization.Currencies);

            //if (data.Globalization.Countries != null)
            //{
            //    foreach (var c in data.Globalization.Countries)
            //    {
            //        countrySeedRespository.AddOrMerge(c);
            //    }
            //}

            AddOrMerge(countrySeedRespository, data.Globalization.Countries);

            //if (data.Globalization.Languages != null)
            //{
            //    foreach (var l in data.Globalization.Languages)
            //    {
            //        languageSeedRespository.AddOrMerge(l);
            //    }
            //}

            AddOrMerge(languageSeedRespository, data.Globalization.Languages);

            //if (data.Websites != null)
            //{
            //    foreach (var w in data.Websites)
            //    {
            //        websiteSeedRespository.AddOrMerge(w);
            //    }
            //}

            AddOrMerge(websiteSeedRespository, data.Websites);


            //if (data.Assortments != null)
            //{
            //    foreach (var a in data.Assortments)
            //    {
            //        assortmentSeedRespository.AddOrMerge(a);
            //    }
            //}

            AddOrMerge(assortmentSeedRespository, data.Assortments);

            if (data.Globalization.Channels != null)
            {
                foreach (var c in data.Globalization.Channels)
                {
                    channelSeedRespository.AddOrMerge(c);
                    if (c.Countries != null)
                    {
                        foreach (var cc in c.Countries)
                        {
                            countrySeedRespository.AddOrMerge(cc);
                        }
                    }
                }
            }
        }

        private void AddOrMerge<T, TSeedGenerator>(Repository<T, TSeedGenerator> repository, IEnumerable<T> values)
            where T : GraphQlObject
            where TSeedGenerator : ISeedGenerator<T>
        {
            if (values != null)
            {
                foreach (var value in values)
                {
                    repository.AddOrMerge(value);
                }
            }
        }
    }
}
