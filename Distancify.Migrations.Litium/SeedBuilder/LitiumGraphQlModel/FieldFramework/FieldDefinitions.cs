﻿using Distancify.Migrations.Litium.SeedBuilder.LitiumGraphQlModel.FieldFramework.Definitions;
using System.Collections.Generic;

namespace Distancify.Migrations.Litium.SeedBuilder.LitiumGraphQlModel.FieldFramework
{
    public class FieldDefinitions
    {
        public IEnumerable<FieldDefinition> Primitives { get; set; }
        public IEnumerable<TextOptionFieldDefinition> TextOptions { get; set; }
        public IEnumerable<PointerFieldDefinition> Pointers { get; set; }
        public IEnumerable<MultiFieldDefinition> MultiFields { get; set; }
        public IEnumerable<DecimalOptionFieldDefinition> DecimalOptions { get; set; }
        public IEnumerable<IntOptionFieldDefinition> IntOptions { get; set; }
    }
}
