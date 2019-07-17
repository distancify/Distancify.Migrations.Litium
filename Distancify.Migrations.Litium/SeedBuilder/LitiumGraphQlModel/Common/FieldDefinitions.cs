using System.Collections.Generic;

namespace Distancify.Migrations.Litium.SeedBuilder.LitiumGraphQlModel.Common
{
    public class FieldDefinitions
    {
        public IEnumerable<FieldDefinition> Primitives { get; set; }
        public IEnumerable<TextOptionFieldDefinition> TextOptions { get; set; }
        public IEnumerable<PointerFieldDefinition> Pointers { get; set; }
        public IEnumerable<MultiFieldDefinition> MultiFields { get; set; }
        public IEnumerable<DecimalOptionFieldDefinition> DecimalOptions { get; set; }
    }
}
