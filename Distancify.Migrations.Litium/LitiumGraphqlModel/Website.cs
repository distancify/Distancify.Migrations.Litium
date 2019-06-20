using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distancify.Migrations.Litium.LitiumGraphqlModel
{
    public class Website
    {
        public string Id { get; set; }
        public Guid SystemId { get; set; }

        public WebsiteFieldTemplate FieldTemplate { get; set; }
    }
}
