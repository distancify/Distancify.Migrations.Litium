using Distancify.Migrations.Litium.SeedBuilder.LitiumGraphQlModel;
using Distancify.Migrations.Litium.Seeds;
using System;
using System.Collections.Generic;
using System.Text;
using Distancify.Migrations.Litium.SeedBuilder.Repositories;

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

            if (data.Globalization?.Languages != null)
            {
                _languageSeedRepository.WriteMigration(migrationBuilder);
            }

            if (data.Products?.UnitOfMeasurements != null)
            {
                _unitOfMeasurementRepository.WriteMigration(migrationBuilder);
            }

            if (data.Products?.Inventories != null)
            {
                _inventoryRepository.WriteMigration(migrationBuilder);
            }

            if (data.Products?.Assortments != null)
            {
                _assortmentSeedRepository.WriteMigration(migrationBuilder);
            }

            if (data.Globalization?.Currencies != null)
            {
                _currencySeedRepository.WriteMigration(migrationBuilder);
            }

            if (data.Globalization?.Countries != null)
            {
                _countrySeedRepository.WriteMigration(migrationBuilder);
            }

            if (data.Globalization?.DomainNames != null)
            {
                _domainNameSeedRepository.WriteMigration(migrationBuilder);
            }

            if (data.Globalization?.Markets != null)
            {
                _marketRepository.WriteMigration(migrationBuilder);
            }

            if (data.Websites?.Websites != null)
            {
                _websiteSeedRepository.WriteMigration(migrationBuilder);
            }

            if (data.Globalization?.Channels != null)
            {
                _channelSeedRepository.WriteMigration(migrationBuilder);
            }

            if (data.Websites?.Websites != null)
            {
                _pageSeedRepository.WriteMigration(migrationBuilder);
            }

            //TODO: This is tied to a channel, but it probably must be seeded first.
            if (data.Globalization?.FieldDefinitions != null)
            {
                _fieldDefinitionRepository.WriteMigration(migrationBuilder);
            }

            return migrationBuilder.ToString();
        }


        public void PopulateSeedsWithData(LitiumGraphQlModel.Data data)
        {
            this.data = data;

            if (data.Globalization != null)
            {
                AddOrMerge(_fieldDefinitionRepository, data.Globalization.FieldDefinitions);
                AddOrMerge(_domainNameSeedRepository, data.Globalization.DomainNames);
                AddOrMerge(_currencySeedRepository, data.Globalization.Currencies);
                AddOrMerge(_marketRepository, data.Globalization.Markets);
                AddOrMerge(_countrySeedRepository, data.Globalization.Countries);
                AddOrMerge(_languageSeedRepository, data.Globalization.Languages);
                AddOrMerge(_channelSeedRepository, data.Globalization.Channels,
                    channel =>
                    {
                        AddOrMerge(_countrySeedRepository, channel.Countries);
                        //AddOrMerge(null, channel.FieldTemplate);
                    });
            }

            if (data.Products != null)
            {
                AddOrMerge(_assortmentSeedRepository, data.Products.Assortments);
                AddOrMerge(_unitOfMeasurementRepository, data.Products.UnitOfMeasurements);
                AddOrMerge(_inventoryRepository, data.Products.Inventories);
            }

            if (data.Websites != null)
            {
                AddOrMerge(_websiteSeedRepository, data.Websites.Websites,
                    website => AddOrMerge(_pageSeedRepository, website.Pages));
                AddOrMerge(_fieldDefinitionRepository, data.Websites.FieldDefinitions);
            }

            if (data.Blocks?.FieldDefinitions != null)
            {
                AddOrMerge(_fieldDefinitionRepository, data.Blocks.FieldDefinitions);
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
