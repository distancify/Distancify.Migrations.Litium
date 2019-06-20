using Distancify.Migrations.Litium.Globalization;
using Distancify.Migrations.Litium.Products;
using Distancify.Migrations.Litium.Websites;
using System.Collections.Generic;

namespace Distancify.Migrations.Litium.LitiumGraphqlModel
{
    public class Data
    {
        public IEnumerable<Channel> Channels { get; set; }

        public IEnumerable<DomainName> DomainNames { get; set; }

        public IEnumerable<Currency> Currencies { get; set; }

        public IEnumerable<Country> Countries { get; set; }
        public IEnumerable<Website> Websites { get; set; }
        public IEnumerable<Assortment> Assortments { get; set; }
        public IEnumerable<Language> Languages { get; set; }

        public void PopulateSeedsWithData(List<ISeed> seeds)
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
            //LanguageSeed
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
            // ChannelSeed
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

            if (DomainNames != null)
            {
                foreach (var d in DomainNames)
                {
                    seeds.Add(new DomainNameSeed(d));
                }
            }

          

            if (Currencies != null)
            {
                foreach (var c in Currencies)
                {
                    seeds.Add(new CurrencySeed(c));
                }
            }

            if (Countries != null)
            {
                foreach (var c in Countries)
                {
                    seeds.Add(new CountrySeed(c));
                }
            }

            if (Languages != null)
            {
                foreach (var l in Languages)
                {
                    seeds.Add(new LanguageSeed(l));
                }
            }
            
            if (Websites != null)
            {
                foreach (var w in Websites)
                {
                    seeds.Add(new WebsiteSeed(w));
                }
            }

            if (Assortments != null)
            {
                foreach (var a in Assortments)
                {
                    seeds.Add(new AssortmentSeed(a));
                }
            }
            if (Channels != null)
            {
                foreach (var c in Channels)
                {
                    seeds.Add(new ChannelSeed(c));
                }
            }

        }
    }
}
