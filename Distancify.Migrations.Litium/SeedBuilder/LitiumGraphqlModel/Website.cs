using System;

namespace Distancify.Migrations.Litium.SeedBuilder.LitiumGraphqlModel
{
    public class Website : GraphQlObject
    {
        public Guid SystemId { get; set; }

        public WebsiteFieldTemplate FieldTemplate { get; set; }

    }
}
