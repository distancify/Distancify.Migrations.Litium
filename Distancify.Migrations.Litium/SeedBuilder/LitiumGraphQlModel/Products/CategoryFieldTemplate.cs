﻿using System;
using System.Collections.Generic;

namespace Distancify.Migrations.Litium.SeedBuilder.LitiumGraphQlModel.Products
{
    public class CategoryFieldTemplate : FieldTemplate
    {
        public Guid DisplayTemplateSystemId { get; set; }
        public List<FieldTemplateFieldGroup> FieldGroups { get; set; }
    }
}