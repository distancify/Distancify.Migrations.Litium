using System;

namespace Distancify.Migrations.Litium.SeedBuilder.LitiumGraphQlModel
{
    public class Website : GraphQlObject
    {
        public Guid SystemId { get; set; }

        public WebsiteFieldTemplate FieldTemplate { get; set; }

    }
}
