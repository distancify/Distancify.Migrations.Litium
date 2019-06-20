using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distancify.Migrations.Litium.Generator
{
    public class Config
    {
        public string Host { get; set; }
        public string Query { get; set; }
        public string Namespace { get; set; }
        public string ClassName { get; set; }
        public string BaseMigration { get; set; }
        public string Output { get; set; }
    }
}
