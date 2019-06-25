using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distancify.Migrations.Litium.Generator
{
    public interface IGenerator
    {
        string GenerateMigration();
    }
}
