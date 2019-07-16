using System.Collections.Generic;

namespace Distancify.Migrations.Litium.SeedBuilder.LitiumGraphQlModel.Websites
{
    public class BlockContainerDefinition : GraphQlObject
    {
        public List<FieldLocalization> Localizations { get; set; }
    }
}
