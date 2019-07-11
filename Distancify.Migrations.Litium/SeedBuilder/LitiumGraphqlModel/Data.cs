﻿using System.Collections.Generic;

namespace Distancify.Migrations.Litium.SeedBuilder.LitiumGraphQlModel
{
    public class Data
    {
        public Globalization.Globalization Globalization { get; set; }
        public IEnumerable<Website> Websites { get; set; }
        public IEnumerable<Assortment> Assortments { get; set; }
    }
}
