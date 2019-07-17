using Distancify.Migrations.Litium.SeedBuilder.Respositories;
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
            get
            {
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

        public void PopulateSeedsWithData(LitiumGraphQlModel.Data data)
        {


            if (data.DomainNames != null)
            {
                foreach (var d in data.DomainNames)
                {
                    domainNameSeedRespository.AddOrMerge(d);
                }
            }



            if (data.Currencies != null)
            {
                foreach (var c in data.Currencies)
                {
                    currencySeedRespository.AddOrMerge(c);
                }
            }

            if (data.Countries != null)
            {
                foreach (var c in data.Countries)
                {
                    countrySeedRespository.AddOrMerge(c);

                }
            }

            if (data.Languages != null)
            {
                foreach (var l in data.Languages)
                {
                    languageSeedRespository.AddOrMerge(l);
                }
            }

            if (data.Websites != null)
            {
                foreach (var w in data.Websites)
                {
                    websiteSeedRespository.AddOrMerge(w);
                }
            }

            if (data.Assortments != null)
            {
                foreach (var a in data.Assortments)
                {
                    assortmentSeedRespository.AddOrMerge(a);
                }
            }

            if (data.Channels != null)
            {
                foreach (var c in data.Channels)
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
    }
}
