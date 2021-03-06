﻿using Distancify.Migrations.Litium.SeedBuilder.LitiumGraphQlModel.FieldFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distancify.Migrations.Litium.SeedBuilder.LitiumGraphQlModel.Blocks
{
    public class Blocks
    {
        public IEnumerable<FieldDefinition> FieldDefinitions { get; set; }
        public IEnumerable<BlockFieldTemplate> BlockFieldTemplates { get; set; }

    }
}
