using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distancify.Migrations.Litium.LitiumGraphqlModel
{
    public class Language : GraphQlObject
    {
        public Guid SystemId { get; set; }

        public bool? IsDefaultLanguage { get; set; }

    }
}
