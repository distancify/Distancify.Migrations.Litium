using Distancify.Migrations.Litium.SeedBuilder.LitiumGraphQlModel;
using Distancify.Migrations.Litium.SeedBuilder.LitiumGraphQlModel.FieldFramework.Definitions;
using Distancify.Migrations.Litium.Seeds.FieldFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distancify.Migrations.Litium.SeedBuilder.Repositories
{
    public class IntOptionFieldDefinitionRepository : Repository<IntOptionFieldDefinition, IntOptionFieldDefinitionSeed>
    {
        protected override IntOptionFieldDefinitionSeed CreateFrom(IntOptionFieldDefinition graphQlItem)
        {
            return IntOptionFieldDefinitionSeed.CreateFrom(graphQlItem);
        }
    }
}
