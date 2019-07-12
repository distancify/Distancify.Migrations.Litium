using Distancify.Migrations.Litium.SeedBuilder.LitiumGraphQlModel;
using Distancify.Migrations.Litium.Seeds;
using System;
using System.Collections.Generic;
using System.Text;
using Distancify.Migrations.Litium.SeedBuilder.Repositories;
using Microsoft.EntityFrameworkCore.Internal;


namespace Distancify.Migrations.Litium.SeedBuilder
{
    public class Generator : IGenerator
    {
        //public List<ISeed> seeds;
        private FieldDefinitionRepository fieldDefinitionRepository  = new FieldDefinitionRepository();
        private UnitOfMeasurementRepository unitOfMeasurementRepository = new UnitOfMeasurementRepository();
        private InventoryRepository inventoryRepository = new InventoryRepository();
        private ChannelRepository channelSeedRespository = new ChannelRepository();
        private CountryRepository countrySeedRespository = new CountryRepository();
        private DomainNameRepository domainNameSeedRespository = new DomainNameRepository();
        private CurrencyRepository currencySeedRespository = new CurrencyRepository();
        private LanguageRepository languageSeedRespository = new LanguageRepository();
        private WebsiteRepository websiteSeedRespository = new WebsiteRepository();
        private AssortmentRepository assortmentSeedRespository = new AssortmentRepository();

        private LitiumGraphQlModel.Data data;

        public int NumberOfSeeds
        {
            get {
                int seedsCount = 0;
                seedsCount += fieldDefinitionRepository.NumberOfItems;
                seedsCount += unitOfMeasurementRepository.NumberOfItems;
                seedsCount += inventoryRepository.NumberOfItems;
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

            var migrationBuilder = new StringBuilder("\n");

            if (this.data.Globalization?.Languages != null)
            {
                languageSeedRespository.WriteMigration(migrationBuilder);
                migrationBuilder.AppendLine();
            }

            if (this.data.Products?.UnitOfMeasurements != null)
            {
                unitOfMeasurementRepository.WriteMigration(migrationBuilder);
                migrationBuilder.AppendLine();
            }

            if (this.data.Products?.Inventories != null)
            {
                inventoryRepository.WriteMigration(migrationBuilder);
            }

            if (data.Products?.Assortments != null)
            {
                assortmentSeedRespository.WriteMigration(migrationBuilder);
                migrationBuilder.AppendLine();
            }

            if (data.Globalization?.Countries != null)
            {
                currencySeedRespository.WriteMigration(migrationBuilder);
                migrationBuilder.AppendLine();
            }

            if (this.data.Globalization?.Countries != null)
            {
                countrySeedRespository.WriteMigration(migrationBuilder);
            }


            if (this.data.Globalization?.FieldDefinitions != null)
            {
                fieldDefinitionRepository.WriteMigration(migrationBuilder);
                migrationBuilder.AppendLine();
            }


            return migrationBuilder.ToString();

            domainNameSeedRespository.WriteMigration(migrationBuilder);
            migrationBuilder.AppendLine();
            websiteSeedRespository.WriteMigration(migrationBuilder);
            migrationBuilder.AppendLine();
            channelSeedRespository.WriteMigration(migrationBuilder);


            return migrationBuilder.ToString();
        }

       
        public void PopulateSeedsWithData(LitiumGraphQlModel.Data data)
        {
            this.data = data;

            //if (data.Globalization.DomainNames != null)
            //{
            //    foreach (var d in data.Globalization.DomainNames)
            //    {
            //        domainNameSeedRespository.AddOrMerge(d);
            //    }
            //}

            if (data.Globalization != null)
            {
                AddOrMerge(fieldDefinitionRepository, data.Globalization.FieldDefinitions);
                AddOrMerge(domainNameSeedRespository, data.Globalization.DomainNames);
                AddOrMerge(currencySeedRespository, data.Globalization.Currencies);
                AddOrMerge(countrySeedRespository, data.Globalization.Countries);
                AddOrMerge(languageSeedRespository, data.Globalization.Languages);
                AddOrMerge(channelSeedRespository, data.Globalization.Channels,
                    channel =>
                    {
                        AddOrMerge(countrySeedRespository, channel.Countries);
                        //AddOrMerge(null, channel.FieldTemplate);
                    });
            }

            if (data.Products != null)
            {
                AddOrMerge(assortmentSeedRespository, data.Products.Assortments);
                AddOrMerge(unitOfMeasurementRepository, data.Products.UnitOfMeasurements);
                AddOrMerge(inventoryRepository, data.Products.Inventories);
            }

            
            AddOrMerge(websiteSeedRespository, data.Websites);
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
