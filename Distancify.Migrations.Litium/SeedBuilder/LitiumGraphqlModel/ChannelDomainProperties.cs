namespace Distancify.Migrations.Litium.SeedBuilder.LitiumGraphqlModel
{
    public class ChannelDomainProperties : GraphQlObject
    {
        public bool? Redirect { get; set; }
        public string UrlPrefix { get; set; }
        public DomainName Domain { get; set; }
    }
}