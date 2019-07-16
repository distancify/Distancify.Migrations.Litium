using Distancify.Migrations.Litium.SeedBuilder.LitiumGraphQlModel;
using Distancify.Migrations.Litium.Seeds;
using System;
using System.Collections.Generic;
using System.Text;
using Distancify.Migrations.Litium.SeedBuilder.Repositories;
using Distancify.Migrations.Litium.SeedBuilder.Repositories.Websites;

namespace Distancify.Migrations.Litium.SeedBuilder
{
    public class Generator : IGenerator
    {
        //public List<ISeed> seeds;
        private readonly FieldDefinitionRepository _fieldDefinitionRepository = new FieldDefinitionRepository();
        private readonly UnitOfMeasurementRepository _unitOfMeasurementRepository = new UnitOfMeasurementRepository();
        private readonly InventoryRepository _inventoryRepository = new InventoryRepository();
        private readonly MarketRepository _marketRepository = new MarketRepository();
        private readonly ChannelRepository _channelSeedRepository = new ChannelRepository();
        private readonly CountryRepository _countrySeedRepository = new CountryRepository();
        private readonly DomainNameRepository _domainNameSeedRepository = new DomainNameRepository();
        private readonly CurrencyRepository _currencySeedRepository = new CurrencyRepository();
        private readonly LanguageRepository _languageSeedRepository = new LanguageRepository();
        private readonly WebsiteRepository _websiteSeedRepository = new WebsiteRepository();
        private readonly AssortmentRepository _assortmentSeedRepository = new AssortmentRepository();
        private readonly PageRepository _pageSeedRepository = new PageRepository();
        private readonly ChannelFieldTemplateRepository _channelFieldTemplateSeedRepository = new ChannelFieldTemplateRepository();
        private readonly BlockFieldTemplateRepository _blockFieldTemplateSeedRepository = new BlockFieldTemplateRepository();
        private readonly MarketFieldTemplateRepository _marketFieldTemplateSeedRepository = new MarketFieldTemplateRepository();
        private readonly PageFieldTemplateRepository _pageFieldTemplateSeedRepository = new PageFieldTemplateRepository();
        private readonly WebsiteFieldTemplateRepository _websiteFieldTemplateSeedRepository = new WebsiteFieldTemplateRepository();


        private LitiumGraphQlModel.Data data;

        public int NumberOfSeeds
        {
            get {
                int seedsCount = 0;
                seedsCount += _fieldDefinitionRepository.NumberOfItems;
                seedsCount += _unitOfMeasurementRepository.NumberOfItems;
                seedsCount += _inventoryRepository.NumberOfItems;
                seedsCount += _marketRepository.NumberOfItems;
                seedsCount += _channelSeedRepository.NumberOfItems;
                seedsCount += _countrySeedRepository.NumberOfItems;
                seedsCount += _domainNameSeedRepository.NumberOfItems;
                seedsCount += _currencySeedRepository.NumberOfItems;
                seedsCount += _languageSeedRepository.NumberOfItems;
                seedsCount += _websiteSeedRepository.NumberOfItems;
                seedsCount += _assortmentSeedRepository.NumberOfItems;
                seedsCount += _pageSeedRepository.NumberOfItems;
                seedsCount += _channelFieldTemplateSeedRepository.NumberOfItems;
                seedsCount += _blockFieldTemplateSeedRepository.NumberOfItems;
                seedsCount += _marketFieldTemplateSeedRepository.NumberOfItems;
                seedsCount += _pageFieldTemplateSeedRepository.NumberOfItems;
                seedsCount += _websiteFieldTemplateSeedRepository.NumberOfItems;
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

            var migrationBuilder = new StringBuilder();

            _channelFieldTemplateSeedRepository.WriteMigration(migrationBuilder);

            _marketFieldTemplateSeedRepository.WriteMigration(migrationBuilder);

            _blockFieldTemplateSeedRepository.WriteMigration(migrationBuilder);

            _pageFieldTemplateSeedRepository.WriteMigration(migrationBuilder);

            _fieldDefinitionRepository.WriteMigration(migrationBuilder);

            _websiteFieldTemplateSeedRepository.WriteMigration(migrationBuilder);

            _languageSeedRepository.WriteMigration(migrationBuilder);

            _unitOfMeasurementRepository.WriteMigration(migrationBuilder);

            _inventoryRepository.WriteMigration(migrationBuilder);

            _assortmentSeedRepository.WriteMigration(migrationBuilder);

            _currencySeedRepository.WriteMigration(migrationBuilder);

            _countrySeedRepository.WriteMigration(migrationBuilder);

            _domainNameSeedRepository.WriteMigration(migrationBuilder);

            _marketRepository.WriteMigration(migrationBuilder);

            _websiteSeedRepository.WriteMigration(migrationBuilder);

            _channelSeedRepository.WriteMigration(migrationBuilder);

            _pageSeedRepository.WriteMigration(migrationBuilder);

            return migrationBuilder.ToString();
        }


        public void PopulateSeedsWithData(LitiumGraphQlModel.Data data)
        {
            this.data = data;

            if (data.Common != null)
            {
                AddOrMerge(_fieldDefinitionRepository, data.Common.FieldDefinitions);
            }

            if (data.Globalization != null)
            {
                AddOrMerge(_channelFieldTemplateSeedRepository, data.Globalization.ChannelFieldTemplates);
                AddOrMerge(_domainNameSeedRepository, data.Globalization.DomainNames);
                AddOrMerge(_currencySeedRepository, data.Globalization.Currencies);
                AddOrMerge(_marketRepository, data.Globalization.Markets);
                AddOrMerge(_marketFieldTemplateSeedRepository, data.Globalization.MarketFieldTemplates);
                AddOrMerge(_countrySeedRepository, data.Globalization.Countries);
                AddOrMerge(_languageSeedRepository, data.Globalization.Languages);
                AddOrMerge(_channelSeedRepository, data.Globalization.Channels);
            }

            if (data.Products != null)
            {
                AddOrMerge(_assortmentSeedRepository, data.Products.Assortments);
                AddOrMerge(_unitOfMeasurementRepository, data.Products.UnitOfMeasurements);
                AddOrMerge(_inventoryRepository, data.Products.Inventories);
            }

            if (data.Websites != null)
            {
                AddOrMerge(_websiteFieldTemplateSeedRepository, data.Websites.WebsiteFieldTemplates);
                AddOrMerge(_pageFieldTemplateSeedRepository, data.Websites.PageFieldTemplates);
                AddOrMerge(_websiteSeedRepository, data.Websites.Websites,
                    website => AddOrMerge(_pageSeedRepository, website.Pages));
            }

            if (data.Blocks != null)
            {
                AddOrMerge(_blockFieldTemplateSeedRepository, data.Blocks.BlockFieldTemplates);
            }
        }

        private void AddOrMerge<T, TSeedGenerator>(Repository<T, TSeedGenerator> repository, IEnumerable<T> values, Action<T> nestedPopulation = null)
            where T : GraphQlObject
            where TSeedGenerator : ISeedGenerator<T>
        {
            if (values != null)
            {
                foreach (var value in values)
                {
                    repository.AddOrMerge(value);
                    nestedPopulation?.Invoke(value);
                }
            }
        }
    }
}
