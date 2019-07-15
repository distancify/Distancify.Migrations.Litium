using System;
using System.Collections.Generic;
using Litium.FieldFramework;
using Litium.Websites;

namespace Distancify.Migrations.Litium.SeedBuilder.LitiumGraphQlModel
{
    public class Website : GraphQlObject
    {
        public override string Id  => SystemId.ToString();

        public Guid SystemId { get; set; }

        public WebsiteFieldTemplate FieldTemplate { get; set; }
        public List<FieldLocalization> Localizations { get; set; }

    }
}
