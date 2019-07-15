using System;
using System.Collections.Generic;
using Distancify.Migrations.Litium.SeedBuilder.LitiumGraphQlModel;

namespace Distancify.Migrations.Litium.SeedBuilder.LitiumGraphQlModel
{
    public class FieldDefinition : GraphQlObject
    {
        public Guid? SystemId { get; set; }
        public string FieldType { get; set; }
        public string AreaType { get; set; }

        public bool MultiCulture { get; set; }
        public bool CanBeGridColumn { get; set; }
        public bool CanBeGridFilter { get; set; }

        public List<FieldLocalization> Localizations { get; set; }

        public CompositeFieldOption Option { get; set; }
    }
}
