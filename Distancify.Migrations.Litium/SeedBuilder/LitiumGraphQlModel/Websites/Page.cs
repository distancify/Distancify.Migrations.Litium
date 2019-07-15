using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distancify.Migrations.Litium.SeedBuilder.LitiumGraphQlModel.Websites
{
    public class Page : GraphQlObject
    {
        public Guid SystemId { get; set; }
        public Guid ParentPageSystemId { get; set; }
    }
}
