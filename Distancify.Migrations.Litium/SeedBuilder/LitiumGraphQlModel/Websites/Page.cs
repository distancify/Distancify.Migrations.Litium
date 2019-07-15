﻿using System;
using System.Collections.Generic;

namespace Distancify.Migrations.Litium.SeedBuilder.LitiumGraphQlModel.Websites
{
    public class Page : GraphQlObject
    {
        public override string Id => SystemId.ToString();

        public Guid SystemId { get; set; }
        public Guid ParentPageSystemId { get; set; }
        public Guid WebsiteSystemId { get; set; }


        public List<FieldLocalization> Localizations { get; set; }
        public PageFieldTemplate FieldTemplate { get; set; }
    }
}
