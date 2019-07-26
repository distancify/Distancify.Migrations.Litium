using Distancify.Migrations.Litium.SeedBuilder.LitiumGraphQlModel.Common;
using System;
using System.Collections.Generic;

namespace Distancify.Migrations.Litium.SeedBuilder.LitiumGraphQlModel.Websites
{
    public class Website : GraphQlObject
    {
        public override string Id  => SystemId.ToString();

        public Guid SystemId { get; set; }

        public WebsiteFieldTemplate FieldTemplate { get; set; }
        public List<FieldLocalization> Localizations { get; set; }

        public List<Page> Pages { get; set; }
        public Dictionary<string, Field> Fields { get; set; }
    }
}
