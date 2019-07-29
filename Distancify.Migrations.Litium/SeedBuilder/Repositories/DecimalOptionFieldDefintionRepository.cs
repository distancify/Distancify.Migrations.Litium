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
    public class DecimalOptionFieldDefintionRepository : Repository<DecimalOptionFieldDefinition, DecimalOptionFieldDefinitionSeed>
    {
        protected override DecimalOptionFieldDefinitionSeed CreateFrom(DecimalOptionFieldDefinition graphQlItem)
        {
            return DecimalOptionFieldDefinitionSeed.CreateFrom(graphQlItem);
        }
    }
}
