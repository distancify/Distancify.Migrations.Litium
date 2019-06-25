using Distancify.Migrations.Litium.SeedBuilder.LitiumGraphqlModel;
using System;
using System.Linq;
using System.Text;
using Distancify.Migrations.Litium.Seeds.GlobalizationSeeds;

namespace Distancify.Migrations.Litium.SeedBuilder.Respositories
{
    public class ChannelRepository : Repository<Channel>
    {

        public override void AppendMigration(StringBuilder builder)
        {
            foreach (var channel in Items.Values)
            {

                if (channel == null || string.IsNullOrEmpty(channel.Id))
                {
                    throw new NullReferenceException("At least one Channel with an ID obtained from the GraphQL endpoint is needed in order to ensure the Channels");
                }

                if (channel.FieldTemplate == null)
                {
                    throw new NullReferenceException("Can't ensure channel if no ChannelFieldTemplate is returned from GraphQL endpoint");
                }


                //builder.AppendLine($"\t\t\t{nameof(ChannelSeed)}.{nameof(ChannelSeed.Ensure)}(\"{channel.Id}\", \"\")");
                builder.AppendLine($"\t\t\t{nameof(ChannelSeed)}.{nameof(ChannelSeed.Ensure)}(\"{channel.Id}\", \"{channel.FieldTemplate.Id}\")");
                // WithField
                // WithField

                if (channel.Domains != null && channel.Domains.Count() > 0)
                {
                    foreach (var d in channel.Domains)
                    {
                        if (d.Domain == null)
                        {
                            throw new NullReferenceException("Can't ensure with country link if no Domain is returned from GraphQL endpoint as part of Channel");
                        }
                        builder.Append($"\t\t\t.{nameof(ChannelSeed.WithDomainNameLink)}(\"{d.Domain.Id}\"");

                        //d.Redirect //redirect
                        //d.UrlPrefix //urlPrefix
                        builder.AppendLine(")");
                    }
                }

                // WithoutDomainNameLink
                // WithMarket
                //WithCountryLink
                // WithoutCountryLink
                // WithWebsite
                // ProductLanguage
                // GoogleAnalyticsAccountId
                // GoogleTagManagerContainerId
                // ShowPricesWithVat
                //PriceAgents


                //foreach (var c in channel.CountryLinks)
                //{
                //    builder.AppendLine($"\t\t\t\t.{nameof(ChannelSeed.WithCountryLink)}(\"{c.Id}\")");
                //}

                //AppendFields(i, builder);

                builder.AppendLine("\t\t\t\t.Commit();");

            }


        }
    }
}
