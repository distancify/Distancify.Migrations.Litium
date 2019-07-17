﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distancify.Migrations.Litium.SeedBuilder.LitiumGraphQlModel
{
    public class TextOption
    {
        public List<TextOptionItem> Items { get; set; }

        public bool MultiSelect { get; set; }
        public bool ManualSort { get; set; }
    }
}